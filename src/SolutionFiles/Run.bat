SET ASPNETCORE_ENVIRONMENT=Development
SET LAUNCHER_PATH=bin\Debug\netcoreapp3.1\Nop.Web.exe
cd /d "C:\Program Files\IIS Express\"
iisexpress.exe /config:"C:\Users\d82kr\source\repos\nopCommerce\src\.vs\NopCommerce\config\applicationhost.config" /site:"Nop.Web" /apppool:"Nop.Web AppPool
