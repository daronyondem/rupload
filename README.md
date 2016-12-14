rupload
=======

This is a OS wide context menu - right click and *r*apid *upload* - tool targeting Windows Azure Storage. 

There is no way the app is ready for prime time, however I'm currently using it (works) on my machine. 

#Setup
1. Compile the app.
1. You will get a JSON config file *(rupload_azureconfig.json)* in *C:\Users\UserName\AppData\Roaming*. Add you Azure Storage account credentials there and change `UseDevelopmentStorage` value to `false`.
1. Copy the whole thing into a folder you desire.
2. Edit the registry file to point your installation folder and run.
 * Add a new key under `HKEY_CLASSES_ROOT\*\shell` with the name "Rupload" and "command" key. Add `"C:\YourPath\rupload.exe" "%1"` to default string data as a value. [See.](https://i.imgsafe.org/6246f38fc5.png)
3. Hopefully.. Voila!

#How to use?
1. Right click any file.
1. Click "Rupload"
1. When the upload is complete the app will shutdown and you will have a public link in your clipboard.
