# Project Structure

## Repository layout

```
v4posme_PrinterBarCode/              # Solution root (contains .sln, .vs)
  packages/                          # Restored NuGet packages (classic)
  v4posme_PrinterBarCode/            # The project folder
    Program.cs                       # Entry point: TLS setup, config load, launch MainForm
    config.json                      # Runtime configuration (copied to output)
    App.config / packages.config     # Framework + NuGet manifests
    v4posme_PrinterBarCode.csproj    # MSBuild project (explicit Compile includes)
    Forms/                           # Windows Forms UI
    Models/                          # Plain data/config classes
    Services/                        # Business logic (networking, printing, config, logging)
    Properties/                      # AssemblyInfo
    bin/ obj/ dist/                  # Build output (not source)
```

## Layered organization

- **Program.cs** — composition root. Configures security protocol, wires global exception handlers, loads `AppConfig`, initializes `Logger`, then runs `MainForm`.
- **Models/** — POCO data holders only (`AppConfig`, `BarcodeConfig`, `Product`). JSON-bound with `[JsonProperty]`. Presentation/derived helpers use `[JsonIgnore]` computed properties (e.g. `Product.EffectiveBarcode`, `Product.Code`). No behavior/IO here.
- **Services/** — all logic. Each service has one responsibility:
  - `ConfigService` (static) — loads/validates `config.json`.
  - `ProductService` — downloads and parses the product list; tolerant JSON extraction.
  - `BarcodePrinter` — renders and prints labels via `System.Drawing.Printing`.
  - `Code128Encoder` (static) — pure Code 128 encoding to bar/space module widths.
  - `Logger` (static) — thread-safe file logging.
- **Forms/** — UI only. Each form has a hand-written `.cs` and a generated `.Designer.cs` (keep the `DependentUpon` relationship in the csproj). `MainForm` receives `AppConfig` via constructor.

## Conventions

- Namespace root is `v4posme_PrinterBarCode`; sub-namespaces mirror folders (`.Forms`, `.Models`, `.Services`).
- Dependencies flow one way: Forms → Services → Models. Services depend on config/models, never on Forms.
- Pass `AppConfig` by constructor injection into services (`new ProductService(config)`), rather than reading globals.
- Stateless utilities are `static` classes; services that hold config are instance classes.
- Comments and user/log messages are in Spanish; match the surrounding style.
- This is a classic (non-SDK) csproj: when adding a file you must add an explicit `<Compile Include="...">` entry. New forms need both the `.cs` and `.Designer.cs` entries with `DependentUpon`.
- Do not edit anything under `bin/`, `obj/`, `dist/`, or `packages/`.
