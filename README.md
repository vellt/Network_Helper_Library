# NetworkHelper.dll

- A NetworkHelper egy könyvtár, amely megkönnyíti a backend kommunikációt .NET Framework, C# projeketben.
- Függősége: Newtonsoft.Json (13.0.3), amely a hálózati hívás során kapott JSON adatok, Objektumokká történő deserializálásáért felel.
- Verzió: v0.0.2
- Támogatottság: .NET Framework 4.7.2 vagy újjabb

## Első lépések
- A repo releases fülén található kettő dll-t (könyvtárat) töltse le. Amennyiben nem találja az alábbi linkeken közvetlenül is megteheti:
    - [NetworkHelper.dll](https://github.com/vellt/Network_Helper_Library/releases/download/v0.0.3/NetworkHelper.dll)
    - [Newtonsoft.Json.dll](https://github.com/vellt/Network_Helper_Library/releases/download/v0.0.3/Newtonsoft.Json.dll)
- Ezt követően a .NET-es projekt (Visual Studio) solution explorerjében lévő "References"-re jobb klikk, majd "Add Refenence", ekkor betöltődik egy ablak, ahol bal lent lévő gombok közül kattintson a "Browse..." felíratú gombra. A fájlkezelő segítségével tallózza be a korábban letöltött kettő dll-t.
- Ha lenyitja a solution explorerben lévő "References" fület, láthatja, hogy hozzáadásra került a kettő könyvtár (dll)


<br><br>

------------------
# [MINTA BACKENDDÉRT KATTOLJ IDE](https://github.com/vellt/Network_Helper_Library/blob/master/minta_backend.js)
---------------

<br><br>

# Backend statikus osztály használata

## A könyvtárban az alábbi `HTTP kérések` elérhetőek:
    | Metódus | Leírás                                     
    |---------|-----------------------------------------------------------------------------------
    | GET     | adat olvasás (fetch)                       
    | POST    | adat létrehozás                            
    | PUT     | adat módosítás                             
    | DELETE  | adat törlés                    


<br><br>

## `GET` kérés kiépítse
```C#
string url = "http://localhost:3000/students";
Backend.GET(url).Send();
```

-------------

## `POST` kérés kiépítése
> A body tartalma: `Osztály` típusú objektum. Mely egy opcionális láncolat. Nem kötelező eleme a kérés elküldésének. Az osztálynak egy-egy adatbázisbéli táblát kell reprezentálnia. Itt a kulcsok a property (tulajonság) nevének megfelelően fognak elküldődni. Ezért érdemes az osztály property-ket karakterpontosan elnevezni.
```C#
string url = "http://localhost:3000/students";
Student student = new Student { phone="12132", name="Sanyi", email="email" };
Backend.POST(url).Body(student).Send();
```

-------------

## `PUT` kérés kiépítése
> A body tartalma: `Osztály` típusú objektum. Mely egy opcionális láncolat. Nem kötelező eleme a kérés elküldésének. Az osztálynak egy-egy adatbázisbéli táblát kell reprezentálnia. Itt a kulcsok a property (tulajonság) nevének megfelelően fognak elküldődni. Ezért érdemes az osztály property-ket karakterpontosan elnevezni.
```C#
string url = "http://localhost:3000/students";
Student student = new Student { id = 11, name="Bela" };
Backend.PUT(url).Body(student).Send();
```

-------------

## `DELETE` kérés kiépítése
#### Body-val történő azonosítás
> A body tartalma: `Osztály` típusú objektum. Mely egy opcionális láncolat. Nem kötelező eleme a kérés elküldésének. Az osztálynak egy-egy adatbázisbéli táblát kell reprezentálnia. Itt a kulcsok a property (tulajonság) nevének megfelelően fognak elküldődni. Ezért érdemes az osztály property-ket karakterpontosan elnevezni.
```C#
string url = "http://localhost:3000/students";
Backend.DELETE(url).Body(new Student { id = 11 }).Send();
```
#### URL paraméteres azonosítás
> Ebben az esetben már nincs szükségünk a body láncolatra, hiszen az URL tartalmazza az azonosítóját a törlésre szánt entiásnak.
```C#
string url = "http://localhost:3000/students/1";
Backend.DELETE(url).Send();
```

<br><br>

## Response-ból adatkinyerés
### `ToList` publikus függvénnyel
> Visszatérési értéke listbába rendezett Osztály objektumok, melyek a fetch-elt adatokból képződnek. A generitikusan megadott Osztály típus tulajonság neveinek karakterpontosnak kell lenniük az adatbázis mezőivel, mivel háttérben Json deserializálás történik.
```C#
List<Student> students = Backend.GET(url).Send().ToList<Student>();
```

### `Message` publikus tulajdonsággal
> a backendtől visszakapott üzenetet tudjuk kinyerni, például kiírathatjuk, hogy `Sikeres Törlés!` vagy `Hiba!`
```C#
Console.WriteLine(Backend.DELETE(url).Body(new Student { id = 12 }).Send().Message);
```

### `StatusCode` publikus tulajdonsággal
> Visszakapjuk, hogy a kérés milyen státuszkóddal tért vissza. (OK==200, stb stb..)
```C#
Student student = new Student { phone="12132", name="Sanyi", email="email" };
Response response = Backend.POST(url).Body(student).Send();
if(response.StatusCode == StatusCode.OK) Console.WriteLine(response.Message);
```

<br><br>

# forráskód
[https://github.com/vellt/Network_Helper_Library/blob/master/NetworkHelper/Backend.cs](https://github.com/vellt/Network_Helper_Library/blob/master/NetworkHelper/Backend.cs)


# A könyvtár Szerkezete
![](https://raw.githubusercontent.com/vellt/Network_Helper_Library/master/ClassDiagram.png)
