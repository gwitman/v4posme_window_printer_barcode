# Product

v4posme_PrinterBarCode is a Windows desktop utility that downloads a product catalog from a POSME backend and prints barcode labels on thermal label printers.

## Core purpose

- Fetch products (code, barcode, name, public price, quantity) from a configurable POSME API endpoint.
- Let the user pick products and a print quantity per item via a Windows Forms UI.
- Render and print barcode labels (Code 39 font and/or Code 128 drawn barcodes) sized for thermal label stock.

## Key behaviors

- Configuration is external and file-based (`config.json`), copied next to the executable. Users adjust the API URL, credentials, printer, and label layout without recompiling.
- A "priority printer" in config always overrides the user's printer selection.
- Label layout defaults to a 2 x 1 inch (50.8 x 25.4 mm) thermal label; name and price display are toggleable.
- All significant actions and errors are written to a text log for field troubleshooting.

## Audience and context

- Target users are non-technical store staff on Windows machines.
- The app runs in environments where the backend sits behind a WAF, so HTTP behavior (User-Agent, TLS version, headers) is tuned to pass those filters.
- User-facing text, code comments, and log messages are written in Spanish. Keep this convention.
