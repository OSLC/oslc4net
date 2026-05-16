# OSLC4Net Documentation

This directory contains the source files for the OSLC4Net documentation site, which is published to [oslc4net.github.io](https://oslc4net.github.io).

## Structure

```
docs/
├── docfx.json          # DocFX configuration
├── index.md            # Documentation home page
├── toc.yml             # Top-level table of contents
├── articles/           # Documentation articles
│   ├── toc.yml
│   └── getting-started.md
├── images/             # Images and assets
├── template/           # Custom DocFX template
│   └── public/
│       └── main.js
└── .nojekyll           # Prevents Jekyll processing on GitHub Pages
```

## Building Locally

To build the documentation locally:

```bash
# Install DocFX (if not already installed)
dotnet tool install -g docfx

# Build the documentation
cd docs
docfx docfx.json

# Serve locally (with live reload)
docfx docfx.json --serve
```

The documentation will be available at http://localhost:8080

## CI/CD

The documentation is automatically built and deployed when changes are pushed to the `main` branch:

1. **Build trigger**: Changes to `docs/**`, `OSLC4Net_SDK/**/*.cs`, or `OSLC4Net_SDK/**/*.csproj`
2. **Workflow**: `.github/workflows/docs.yml`
3. **Deployment**: Built site is pushed to the [oslc4net.github.io](https://github.com/oslc4net/oslc4net.github.io) repository
4. **Requirements**: `DOCS_DEPLOY_TOKEN` secret must be configured with write access to oslc4net.github.io

## Adding Content

### New Articles

1. Create a new `.md` file in the `articles/` directory
2. Add an entry to `articles/toc.yml`
3. Commit and push to trigger the build

### API Documentation

API documentation is automatically generated from XML documentation comments in the source code. To improve API docs:

1. Add XML documentation comments to public APIs in `OSLC4Net_SDK/`
2. Build will automatically include updated API reference

## Troubleshooting

### Build fails with "destination already exists"

Clear the `_site` directory:
```bash
rm -rf docs/_site
```

### Changes not appearing on the site

1. Check the GitHub Actions workflow status
2. Verify the `DOCS_DEPLOY_TOKEN` is valid and has write permissions
3. Check the oslc4net.github.io repository for the deployment commit

## More Information

- [DocFX documentation](https://dotnet.github.io/docfx/)
- [OSLC specifications](https://open-services.net/specifications/)
