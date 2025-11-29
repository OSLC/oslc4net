# Documentation Deployment Setup

This guide explains how to set up the `DOCS_DEPLOY_TOKEN` secret required for deploying documentation from the OSLC/oslc4net repository to oslc4net.github.io.

## Overview

The documentation workflow (`.github/workflows/docs.yml`) builds the DocFX documentation from sources in this repository and deploys the built site to the `oslc4net.github.io` repository using a GitHub Personal Access Token (PAT).

## Setup Instructions

### Option 1: Personal Access Token (Recommended)

1. **Create a Personal Access Token**:
   - Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Or visit: https://github.com/settings/tokens
   - Click "Generate new token (classic)"
   - Give it a descriptive name: `OSLC4Net Docs Deploy`
   - Set expiration as needed (recommend: 1 year)
   - Select scopes:
     - ✅ `repo` (Full control of private repositories)
       - This includes `public_repo` for public repositories
   - Click "Generate token"
   - **Copy the token immediately** (you won't see it again!)

2. **Add the secret to the OSLC/oslc4net repository**:
   - Go to the OSLC/oslc4net repository settings
   - Navigate to: Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `DOCS_DEPLOY_TOKEN`
   - Value: Paste the PAT you just created
   - Click "Add secret"

### Option 2: Deploy Key (Alternative)

If you prefer not to use a PAT:

1. **Generate an SSH key pair**:
   ```bash
   ssh-keygen -t ed25519 -C "docs-deploy-key" -f ~/.ssh/oslc4net-docs-deploy
   # Don't set a passphrase (press Enter)
   ```

2. **Add the public key to oslc4net.github.io**:
   - Go to oslc4net.github.io repository settings
   - Navigate to: Settings → Deploy keys
   - Click "Add deploy key"
   - Title: `OSLC/oslc4net docs deployment`
   - Key: Paste contents of `~/.ssh/oslc4net-docs-deploy.pub`
   - ✅ Check "Allow write access"
   - Click "Add key"

3. **Add the private key to OSLC/oslc4net**:
   - Go to OSLC/oslc4net repository settings
   - Navigate to: Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `DOCS_DEPLOY_KEY`
   - Value: Paste contents of `~/.ssh/oslc4net-docs-deploy` (the private key)
   - Click "Add secret"

4. **Update the workflow**:
   - Edit `.github/workflows/docs.yml`
   - Change `personal_token: ${{ secrets.DOCS_DEPLOY_TOKEN }}`
   - To: `deploy_key: ${{ secrets.DOCS_DEPLOY_KEY }}`

## Verifying the Setup

1. Make a small change to any file in the `docs/` directory
2. Commit and push to the `main` branch
3. Go to Actions tab in OSLC/oslc4net
4. Watch the "Build and Deploy Documentation" workflow
5. Check that the deployment succeeds
6. Visit https://oslc4net.github.io to see your changes (may take a few minutes)

## Troubleshooting

### "remote: Permission to oslc4net/oslc4net.github.io.git denied"

- The token/key doesn't have write access to the target repository
- For PAT: Check that it has `repo` scope
- For Deploy Key: Ensure "Allow write access" was checked

### "ref refs/heads/main not found"

- The target branch might not exist in oslc4net.github.io
- Create an initial commit in that repository first

### Workflow doesn't trigger

- Check the `paths` filter in `.github/workflows/docs.yml`
- Ensure your changes match one of the patterns
- Try manual trigger using "workflow_dispatch"

## Security Notes

- **PATs** grant broad access - they can be used for any repository the account can access
- **Deploy Keys** are scoped to a single repository - more secure but less flexible
- Tokens should be rotated regularly (set expiration dates)
- Never commit tokens or keys to the repository
- Use environment protection rules for production deployments

## Manual Deployment

If you need to manually deploy documentation:

```bash
# Clone both repositories
cd /tmp
git clone https://github.com/OSLC/oslc4net.git
git clone https://github.com/oslc4net/oslc4net.github.io.git

# Build docs
cd oslc4net/docs
docfx docfx.json

# Copy to github.io repo
rm -rf /tmp/oslc4net.github.io/*
cp -r _site/* /tmp/oslc4net.github.io/
cp .nojekyll /tmp/oslc4net.github.io/

# Commit and push
cd /tmp/oslc4net.github.io
git add .
git commit -m "docs: manual deployment"
git push origin main
```

## Additional Resources

- [GitHub Personal Access Tokens Documentation](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
- [GitHub Deploy Keys Documentation](https://docs.github.com/en/developers/overview/managing-deploy-keys)
- [peaceiris/actions-gh-pages Documentation](https://github.com/peaceiris/actions-gh-pages)
