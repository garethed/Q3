rem http://stackoverflow.com/questions/25983430/asp-net-vnext-publish-to-iis-in-windows-server 
rem you need to be able to see \\poolq3\c$

kpm bundle --configuration Release --runtime kre-clr-win-x64.1.0.0-beta3 -o \\poolq3\c$\poolq