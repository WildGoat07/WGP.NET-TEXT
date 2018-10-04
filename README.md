![logo](https://github.com/WildGoat07/WGP.NET-TEXT/blob/master/logo.png)

# WGP.NET-TEXT

Fixing the internal sfml font bug.

There was a bug when using the basic Font class, which caused crashes when calling some of its methods. This way, it will initialize all the characteres (UTF-8) in the memory, and then won't make it crash. (It **may** still crash during the initialization, though -need more tests-).

## Dependencies

* [WildGoatPackage.NET](https://github.com/WildGoat07/WildGoatPackage.NET)
* [SFML.Net - WG fork](https://github.com/WildGoat07/SFML.Net)
