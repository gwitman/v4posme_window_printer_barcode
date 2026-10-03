using Newtonsoft.Json;

namespace v4posme_PrinterBarCode.Models
{
    /// <summary>
    /// Representa un producto (item) tal como lo devuelve el endpoint getDataDownload
    /// en la propiedad ListItem. Los nombres JSON coinciden con
    /// Api_AppMobileApi_GetDataDownloadItemsResponse del proyecto MAUI.
    /// </summary>
    public class Product
    {
        [JsonProperty("itemID")]
        public int ItemId { get; set; }

        /// <summary>Numero de producto, usado como "codigo de producto" visible.</summary>
        [JsonProperty("itemNumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("barCode")]
        public string BarCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("precioPublico")]
        public decimal PrecioPublico { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        // --------------------------------------------------------------------
        // Propiedades de presentacion/compatibilidad usadas por el grid e impresion.
        // --------------------------------------------------------------------

        /// <summary>Codigo de producto mostrado en el grid (itemNumber, o itemID si falta).</summary>
        [JsonIgnore]
        public string Code => !string.IsNullOrWhiteSpace(ItemNumber) ? ItemNumber : ItemId.ToString();

        /// <summary>Alias de codigo de barra para el resto de la aplicacion.</summary>
        [JsonIgnore]
        public string Barcode => BarCode;

        /// <summary>Precio mostrado e impreso.</summary>
        [JsonIgnore]
        public decimal Price => PrecioPublico;

        /// <summary>Cantidad a imprimir seleccionada por el usuario en el dialogo.</summary>
        [JsonIgnore]
        public int PrintQuantity { get; set; } = 1;

        /// <summary>Codigo de barra efectivo; si viene vacio usa el codigo del producto.</summary>
        [JsonIgnore]
        public string EffectiveBarcode =>
            string.IsNullOrWhiteSpace(BarCode) ? Code : BarCode;
    }
}
