# NetworkHelper.dll

- A NetworkHelper egy könyvtár, amely megkönnyíti a backend kommunikációt .NET Framework, C# projeketben.
- Függősége: Newtonsoft.Json (13.0.3), amely a hálózati hívás során kapott JSON adatok, Objektumokká történő deserializálásáért felel.
- Verzió: v0.0.2
- Támogatottság: .NET Framework 4.7.2 vagy újjabb

## Első lépések
- A repo releases fülén található kettő dll-t (könyvtárat) töltse le. Amennyiben nem találja az alábbi linkeken közvetlenül is megteheti:
    - [NetworkHelper.dll](https://github.com/vellt/Network_Helper_Library/releases/download/v0.0.2/NetworkHelper.dll)
    - [Newtonsoft.Json.dll](https://github.com/vellt/Network_Helper_Library/releases/download/v0.0.2/Newtonsoft.Json.dll)
- Ezt követően a .Net-es projekt (Visual Studio) solution explorerjében lévő "References"-re jobb klikk, majd "Add Refenence", ekkor betöltődik egy ablak, ahol bal lett lévő gombok közül kattintson a "Browse..." felíratú gombra.
Ekkor betöltődik a fájlkezelő. Segítségével tallózza be a korábban letöltött kettő dll-t.
- Ha lenyitja a solution explorerben lévő "References" fület, láthatja, hogy hozzáadásra került a kettő könyvtár (dll)

-------------

# Használata

A könyvtárban az alábbi HTTP hívások elérhetőek:
- GET
- POST
- PUT
- DELETE

-------------

## `GET` hívás kiépítse
```C#
string url = "http://localhost:3000/idoutazok";
BackendValasz idoutazokValasz = BackendHivas.Kuldese(url, Methods.GET);
```

-------------

## `POST` hívás kiépítése
### Ha a body tartalma: `Dictionary` {kulcs, érték}.
> Ekkor a kulcsokat a backendnek megfelelően tudjuk megválasztani.
```C#
string url = "http://localhost:3000/utanpotlas";
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url, Methods.POST, new Dictionary<string, string> {
    { "bevitel1", "vezeteknev" },
    { "bevitel2", "keresztnev" },
    { "bevitel3", "0" },
    { "bevitel4", "1900-12-02" },
    { "bevitel5", "default.jpg" },
});
```
### Ha a body tartalma: `Osztály` típusú objektum
> Az osztály egy táblát reprezentál. Itt a kulcsok a property (tulajonság) nevének megfelelően fog elküldődni. Ezért érdemes az osztály property-ket karakterpontosan elnevezni.
```C#
string url = "http://localhost:3000/utanpotlas";
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url, Methods.POST, new Idoutazo {
    neme = 0,
    vezeteknev = "Vin",
    keresztnev = "Dizella",
    kep = "default.jpg",
    szuletesi_datum = DateTime.Now,
});
```
### Ha a body tartalma: string `lista`
> A kulcs értékek háttérben dinamikusan készülnek el, (bevitel**n** | n ∈ [1, lista.length]) mintázattal. Pl.: bevitel1, bevitel2, bevitel**n**. Ebből következik, hogy ennél kifejezetten számít a sorrend. Hiszen aszerint lesznek indexelve.
```C#
string url = "http://localhost:3000/utanpotlas";
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url, Methods.POST, new List<string> {
    "vezeteknev",
    "keresztnev",
    "0",
    "1900-12-02",
    "default.jpg",
});
```
-------------

## BackendValasz-ból adatkinyerés
### `List` publikus függvénnyel
> objektumként kapjuk vissza a fetch-elt adatot. A generitikusan megadott Osztály típus property neveinek karakterpontosnak kell lennie, az adatbázis mezőivel.
```C#
string url = "http://localhost:3000/idoutazok";
BackendValasz idoutazokValasz = BackendHivas.Kuldese(url, Methods.GET);
List<Idoutazo> idoutazok = idoutazokValasz.List<Idoutazo>(); // háttérben Json deserializálás történik
```

> vagy rövidebben, láncolt alakban egyből adatlekérdezés és kiíratás:
```C#
string url = "http://localhost:3000/idoutazok";
BackendHivas.Kuldese(url, Methods.GET)
    .List<Idoutazo>()
    .ForEach(x => Console.WriteLine($"{x.id} {x.TeljesNev()} ({x.szuletesi_datum.Year})"));
```

### `Json` publikus tulajdonsággal
> a backendtől visszakapott JSON-t tudjuk kinyerni
```C#
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url2, Methods.POST, new Dictionary<string, string> {
    { "bevitel1", "vezeteknev" },
    { "bevitel2", "keresztnev" },
    { "bevitel3", "0" },
    { "bevitel4", "1900-12-02" },
    { "bevitel5", "default.jpg" },
});
string json= utanpotlasValasz.Json;
```

-------------

## Hibakezelés
hibakezelést a belső Error property teszi lehetővé, érdemes minden adatkinyeréskor megvizsgálni, hogy hibamentesen tudott-e adatot lehívni. Ekkor már biztonságosan fogunk tudni lekérni a BackendValasz osztálytól adatot. pl.:
```C#
string url = "http://localhost:3000/idohurkok";
BackendValasz idohurkokValasz = BackendHivas.Kuldese(url, Methods.GET);
if (!idohurkokValasz.Error)
{
    List<Idohurok> idohurkok = idohurkokValasz.List<Idohurok>();
    idohurkok.ForEach(x => Console.WriteLine($"{x.id} {x.kezdeti_datum} {x.veg_datum} {x.esemeny_nev}"));
}
```
> vagy egy body-s http hívás
```C#
string url = "http://localhost:3000/utanpotlas";
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url, Methods.POST, new List<string> {
    "vezeteknev",
    "keresztnev",
    "0",
    "1900-12-02",
    "default.jpg",
});
if (!utanpotlasValasz.Error) Console.WriteLine(utanpotlasValasz.Json;
```
