# Gringotts Logo Setup Instructions

## How to add the official Gringotts logo to your ATM:

### Option 1: Download Manually

1. Go to: https://static.wikia.nocookie.net/harrypotter/images/9/96/GringottsLogoDH.png
2. Right-click and "Save image as..."
3. Save it as `gringotts-logo.png` in your `GringottsBankUI/public/` folder

### Option 2: Use PowerShell to download

Run this command from your GringottsBankUI directory:

```powershell
Invoke-WebRequest -Uri "https://static.wikia.nocookie.net/harrypotter/images/9/96/GringottsLogoDH.png" -OutFile "public/gringotts-logo.png"
```

### What the logo integration does:

- ✅ Displays the official Gringotts logo in the ATM header
- ✅ Automatically falls back to the goblin emoji (🏛️) if logo fails to load
- ✅ Properly styled and positioned to match the magical theme
- ✅ Responsive design that works on mobile devices

### File structure should look like:

```
GringottsBankUI/
├── public/
│   ├── gringotts-logo.png  ← Add this file
│   └── vite.svg
├── src/
│   └── components/
│       └── GringottsATM.vue ← Already updated
```

Once you add the logo file, restart your container to see the changes:

```bash
docker-compose restart gringottsbankui
```

The ATM header will now display:
[Logo] GRINGOTTS WIZARDING BANK

If the logo fails to load, it will show:
🏛️ GRINGOTTS WIZARDING BANK
