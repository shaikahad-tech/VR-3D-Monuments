# How to run this project in VS Code

1. Open VS Code.
2. Go to **File -> Open Folder...** and select the `VirtualMuseumVR` folder.
3. In VS Code, open the Extensions panel on the left (or press `Cmd+Shift+X` on Mac / `Ctrl+Shift+X` on Windows).
4. Search for an extension called **Live Server** (by Ritwick Dey) and click **Install**.
5. Once installed, open `VirtualMuseum.html` in your VS Code editor.
6. Right-click anywhere in the code of `VirtualMuseum.html` and select **"Open with Live Server"**. (Alternatively, you can click the "Go Live" button that will appear in the bottom right corner of the VS Code status bar).
7. This will automatically open your default web browser to the correct local address (usually `http://127.0.0.1:5500/VirtualMuseum.html`), and the project will work perfectly!

*Note: You must use an extension like Live Server because this project uses ES Modules (the `import` statements in JavaScript). Web browsers block ES Modules from loading if you just double-click the HTML file (`file://` protocol) for security reasons. A local server is required.*
