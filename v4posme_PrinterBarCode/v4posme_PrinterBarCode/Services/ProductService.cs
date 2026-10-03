using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using v4posme_PrinterBarCode.Models;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Descarga y deserializa el listado de productos desde la URL configurada.
    /// </summary>
    public class ProductService
    {
        private readonly AppConfig _config;

        public ProductService(AppConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Descarga el JSON de productos. Soporta un array directo o un objeto
        /// contenedor con propiedades data/products/items/result.
        /// </summary>
        public async Task<List<Product>> GetProductsAsync()
        {
            var method = (_config.HttpMethod ?? "GET").Trim().ToUpperInvariant();
            Logger.Info($"Descargando productos ({method}) desde: {_config.ProductsUrl}");

            // Replica exacta de como la app MAUI envía la peticion:
            // HttpRequestMessage con FormUrlEncodedContent, sin DefaultRequestHeaders.
            var ua = string.IsNullOrWhiteSpace(_config.UserAgent)
                ? "Mozilla/5.0 (Windows NT; Windows NT 10.0; es-NI) WindowsPowerShell/5.1"
                : _config.UserAgent;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(
                    _config.RequestTimeoutSeconds > 0 ? _config.RequestTimeoutSeconds : 60);

                var httpMethod = method == "POST" ? HttpMethod.Post : HttpMethod.Get;
                var req = new HttpRequestMessage(httpMethod, _config.ProductsUrl);

                // Headers directos en el request (no en DefaultRequestHeaders).
                req.Headers.TryAddWithoutValidation("User-Agent", ua);
                req.Headers.ExpectContinue = false;

                if (method == "POST")
                {
                    var fields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("txtNickname", _config.TxtNickname ?? string.Empty),
                        new KeyValuePair<string, string>("txtPassword", _config.TxtPassword ?? string.Empty)
                    };
                    Logger.Info($"POST con parametros: txtNickname='{_config.TxtNickname}', txtPassword=***");
                    req.Content = new FormUrlEncodedContent(fields);
                }

                Logger.Info($"User-Agent: {ua}");
                var response = await client.SendAsync(req).ConfigureAwait(false);

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Logger.Info($"Respuesta HTTP {(int)response.StatusCode} ({response.StatusCode}). Longitud={json?.Length ?? 0}.");

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"El servidor respondio {(int)response.StatusCode}. Cuerpo (primeros 300 chars): " +
                        (json?.Length > 300 ? json.Substring(0, 300) : json ?? string.Empty));
                    throw new InvalidOperationException(
                        $"El servidor respondio {(int)response.StatusCode} ({response.StatusCode}). " +
                        "Revise el log para ver el detalle del cuerpo.");
                }

                // Registramos solo un extracto para no inflar el log con respuestas grandes.
                var preview = json ?? string.Empty;
                if (preview.Length > 1000) preview = preview.Substring(0, 1000) + "... [truncado]";
                Logger.Info("Datos descargados (extracto): " + preview);

                var products = Parse(json);
                Logger.Info($"Productos cargados: {products.Count}");
                return products;
            }
        }

        /// <summary>
        /// Extrae la lista de productos de una respuesta que puede ser texto suelto
        /// con el JSON embebido (espacios, saltos de linea o texto alrededor).
        /// Tambien detecta respuestas de error del tipo {"error":true,"message":"..."}.
        /// </summary>
        public static List<Product> Parse(string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
                return new List<Product>();

            // La respuesta no siempre es JSON puro: puede traer texto o lineas en
            // blanco alrededor. Extraemos el bloque JSON (objeto o array) embebido.
            var json = ExtractJson(rawResponse);
            if (string.IsNullOrWhiteSpace(json))
            {
                Logger.Warn("La respuesta no contiene un bloque JSON reconocible.");
                return new List<Product>();
            }

            JToken token;
            try
            {
                token = JToken.Parse(json);
            }
            catch (Exception ex)
            {
                Logger.Error("No se pudo interpretar el JSON extraido de la respuesta.", ex);
                return new List<Product>();
            }

            JArray array = null;
            if (token.Type == JTokenType.Object)
            {
                // Detectamos respuesta de error de la API.
                var errorToken = token["error"];
                if (errorToken != null && errorToken.Type == JTokenType.Boolean && errorToken.Value<bool>())
                {
                    var message = token["message"]?.ToString() ?? "Error no especificado por el servidor.";
                    Logger.Error("La API devolvio un error: " + message);
                    throw new InvalidOperationException("El servidor devolvio un error: " + message);
                }
            }

            if (token.Type == JTokenType.Array)
            {
                array = (JArray)token;
            }
            else if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;
                // "listItem" es el nombre real de la propiedad en la respuesta de posme.
                string[] candidates = { "listItem", "ListItem", "data", "products", "productos", "items", "result", "results", "payload" };
                foreach (var key in candidates)
                {
                    var prop = obj[key];
                    if (prop != null && prop.Type == JTokenType.Array)
                    {
                        array = (JArray)prop;
                        break;
                    }
                }

                // Si no encontramos array, buscamos el primer array dentro del objeto.
                if (array == null)
                {
                    foreach (var prop in obj.Properties())
                    {
                        if (prop.Value.Type == JTokenType.Array)
                        {
                            array = (JArray)prop.Value;
                            break;
                        }
                    }
                }
            }

            if (array == null)
                return new List<Product>();

            return array.ToObject<List<Product>>() ?? new List<Product>();
        }

        /// <summary>
        /// Localiza y devuelve el primer bloque JSON balanceado (objeto {} o array [])
        /// dentro de un texto que puede contener caracteres o lineas extra alrededor.
        /// Respeta las comillas y los escapes para no cortar en una llave dentro de un string.
        /// </summary>
        public static string ExtractJson(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            // Buscamos el primer caracter de apertura ( { o [ ).
            int start = -1;
            char open = '\0', close = '\0';
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '{') { start = i; open = '{'; close = '}'; break; }
                if (text[i] == '[') { start = i; open = '['; close = ']'; break; }
            }

            if (start < 0)
                return null;

            int depth = 0;
            bool inString = false;
            bool escape = false;

            for (int i = start; i < text.Length; i++)
            {
                char c = text[i];

                if (inString)
                {
                    if (escape) escape = false;
                    else if (c == '\\') escape = true;
                    else if (c == '"') inString = false;
                    continue;
                }

                if (c == '"') { inString = true; continue; }

                if (c == open) depth++;
                else if (c == close)
                {
                    depth--;
                    if (depth == 0)
                        return text.Substring(start, i - start + 1);
                }
            }

            // Si no se cerro, devolvemos desde el inicio (JToken.Parse reportara el error).
            return text.Substring(start);
        }
    }
}
