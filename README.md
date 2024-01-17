# NetworkHelper.dll

- A NetworkHelper egy könyvtár, amely megkönnyít az adatbázis kommunikációt .NET Framework, C# projeketben.
- Függősége: Newtonsoft.Json, amely a hálózati hívás során kapott JSON Objektumokká történő deserializálásáért felel.

## Első lépések
- az alábbi linken elérhető kettő dll-t (könyvtárat) töltse le
- A solution explorerben lévő "References"-re jobb klikk, majd "Add Refenence", ekkor betöltődik egy ablak, ahol bal lett lévő gombok közül kattintson a "Browse..." felíratú gombra.
Ekkor betöltődik a fájlkezelő, segítségével tallózza be a korábban letöltött kettő dll-t.
- Ha lenyitja a solution explorerben lévő "References" fület, láthatja, hogy hozzáadásra került a kettő könyvtár (dll)

-------------

# Használata

A könyvtárban az alábbi HTTP hívások elérhetőek:
- GET
- POST
- PUT
- DELETE

-------------

## GET hívás kiépítse
```C#
string url = "http://localhost:3000/idoutazok";
BackendValasz idoutazokValasz = BackendHivas.Kuldese(url, Methods.GET);
```

## POST hívás kiépítése

**body tartalma**: Dictinonary (kulcs érték). Ekkor a kulcsokat a backendnek megfelelően tudjuk megválasztani.
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
**body tartalma**: Osztály típus, amely egy táblát reprezentál. Itt a kulcsok a property (tulajonság) nevének megfelelően fog elküldődni. Ezért érdemes az osztály property-ket karakterpontosan elnevezni.
```C#
string url = "http://localhost:3000/utanpotlas";
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url2, Methods.POST, new Idoutazo {
    neme = 0,
    vezeteknev = "Vin",
    keresztnev = "Dizella",
    kep = "default.jpg",
    szuletesi_datum = DateTime.Now,
});
```
**body tartalma**: string lista, a kulcs értékek háttérben dinamikusan készülnek el, (bevitel**n** | n ∈ [1, lista.length]) mintázattal. Pl.: bevitel1, bevitel2, bevitel**n**. Ebből következik, hogy ennél kifejezetten számít a sorrend. Hiszen aszerint lesznek indexelve.
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
ResponseAsObject függvénnyel, objektumként kapjuk vissza a fetch-elt adatot.
```C#
string url = "http://localhost:3000/idoutazok";
BackendValasz idoutazokValasz = BackendHivas.Kuldese(url, Methods.GET);
List<Idoutazo> idoutazok = idoutazokValasz.ResponseAsObject<Idoutazo>(); // háttérben Json deserializálás történik
```

vagy rövidebben, láncolt alakban egyből adatlekérdezés és kiíratás:
```C#
string url = "http://localhost:3000/idoutazok";
BackendHivas.Kuldese(url, Methods.GET)
    .ResponseAsObject<Idoutazo>()
    .ForEach(x => Console.WriteLine($"{x.id} {x.TeljesNev()} ({x.szuletesi_datum.Year})"));
```

ResponseAsJson függvénnyel, a backendtől visszakapott JSON-t tudjuk kinyerni
```C#
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url2, Methods.POST, new Dictionary<string, string> {
    { "bevitel1", "vezeteknev" },
    { "bevitel2", "keresztnev" },
    { "bevitel3", "0" },
    { "bevitel4", "1900-12-02" },
    { "bevitel5", "default.jpg" },
});
string json= utanpotlasValasz.ResponseAsJson();
```

-------------

## Hibakezelés
hibakezeléshez a belső Error property teszi lehetővé, érdemes minden adatkinyeréskor megvizsgálni, hogy hibamentesen tudott-e adatot lehívni. Ekkor már biztonságosan fogunk tudni lekérni az osztálytól adatot. pl.:
```C#
string url = "http://localhost:3000/idohurkok";
BackendValasz idohurkokValasz = BackendHivas.Kuldese(url, Methods.GET);
if (!idohurkokValasz.Error)
{
    List<Idohurok> idohurkok = idohurkokValasz.ResponseAsObject<Idohurok>();
    idohurkok.ForEach(x => Console.WriteLine($"{x.id} {x.kezdeti_datum} {x.veg_datum} {x.esemeny_nev}"));
}
```
vagy 
```C#
string url = "http://localhost:3000/utanpotlas";
BackendValasz utanpotlasValasz = BackendHivas.Kuldese(url, Methods.POST, new List<string> {
    "vezeteknev",
    "keresztnev",
    "0",
    "1900-12-02",
    "default.jpg",
});
if (!utanpotlasValasz.Error) Console.WriteLine(utanpotlasValasz.ResponseAsJson());
```
