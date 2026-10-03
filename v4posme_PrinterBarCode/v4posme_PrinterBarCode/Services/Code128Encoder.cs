using System;
using System.Collections.Generic;
using System.Text;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Codificador Code 128 (subconjunto B/C automatico simple, usamos B para
    /// soportar letras y numeros). Devuelve el patron de barras como secuencia
    /// de anchos de modulos (barra/espacio alternados), listo para dibujar.
    /// </summary>
    public static class Code128Encoder
    {
        // Tabla de patrones Code128: cada entrada son 6 digitos (anchos 1..4)
        // que alternan barra-espacio-barra-espacio-barra-espacio.
        private static readonly string[] Patterns =
        {
            "212222","222122","222221","121223","121322","131222","122213","122312","132212","221213",
            "221312","231212","112232","122132","122231","113222","123122","123221","223211","221132",
            "221231","213212","223112","312131","311222","321122","321221","312212","322112","322211",
            "212123","212321","232121","111323","131123","131321","112313","132113","132311","211313",
            "231113","231311","112133","112331","132131","113123","113321","133121","313121","211331",
            "231131","213113","213311","213131","311123","311321","331121","312113","312311","332111",
            "314111","221411","431111","111224","111422","121124","121421","141122","141221","112214",
            "112412","122114","122411","142112","142211","241211","221114","413111","241112","134111",
            "111242","121142","121241","114212","124112","124211","411212","421112","421211","212141",
            "214121","412121","111143","111341","131141","114113","114311","411113","411311","113141",
            "114131","311141","411131","211412","211214","211232","2331112"
        };

        private const int StartB = 104;
        private const int Stop = 106;

        /// <summary>
        /// Codifica el texto en Code128-B y devuelve la lista de anchos de modulos.
        /// Indices pares = barra (negro), impares = espacio (blanco).
        /// </summary>
        public static List<int> Encode(string data)
        {
            if (data == null) data = string.Empty;

            var codes = new List<int> { StartB };
            int checksum = StartB;

            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i] - 32; // Code B: ASCII 32..126 -> 0..94
                if (value < 0 || value > 94) value = 0; // caracter no soportado -> espacio
                codes.Add(value);
                checksum += value * (i + 1);
            }

            codes.Add(checksum % 103); // digito de control
            codes.Add(Stop);

            // Construimos la secuencia de anchos a partir de los patrones.
            var widths = new List<int>();
            foreach (var code in codes)
            {
                var pattern = Patterns[code];
                foreach (var c in pattern)
                    widths.Add(c - '0');
            }

            return widths;
        }
    }
}
