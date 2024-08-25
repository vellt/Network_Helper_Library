# NetworkHelper.dll

- **A NetworkHelper** egy .NET Framework és C# projektekhez készült könyvtár, amely megkönnyíti a backend kommunikációt. A könyvtár egyszerűsíti az HTTP kérések küldését és a JSON válaszok feldolgozását.
- **Verzió**: v0.1.0
- **Támogatott .NET Verziók**: .NET Framework 4.7.2 vagy újabb

### Első lépések

- **Könyvtár (DLL) letöltése**:
  
  - Töltsd le a `NetworkHelper.dll` fájlt a [GitHub Releases](https://github.com/vellt/Network_helper_Library/releases) oldalról.

- **DLL hozzáadása a projektedhez**:
  
  - Nyisd meg a Visual Studio-t és navigálj a **Solution Explorer** ablakhoz.
  - Jobb klikk a **References** elemre, válaszd az **Add Reference** lehetőséget.
  - A megjelenő ablakban kattints a **Browse...** gombra, és tallózd be a letöltött DLL fájlt.
  - Ellenőrizd, hogy a DLL megjelent a **References** alatt.

---

### Backend Statikus Osztály Használata

A **Backend** statikus osztály segítségével könnyedén létrehozhatsz és küldhetsz HTTP kéréseket.

### Elérhető HTTP Metódusok

| Metódus  | Leírás                | Példa Használat                       |
| -------- | --------------------- | ------------------------------------- |
| `GET`    | Adatok lekérése       | `Backend.GET(url).Send()`             |
| `POST`   | Új adatok létrehozása | `Backend.POST(url).Body(body).Send()` |
| `PUT`    | Adatok módosítása     | `Backend.PUT(url).Body(body).Send()`  |
| `DELETE` | Adatok törlése        | `Backend.DELETE(url).Send()`          |

---

### `GET` Kérés Kiépítése

```csharp
string url = "http://localhost:3000/students"; 
Response response = Backend.GET(url).Send();
```

---

### `POST` Kérés Kiépítése

A `POST` kéréshez meg kell adni a JSON body-t, amely az új objektumot tartalmazza, amit létre szeretnél hozni.  Body felhasználása opcionális. Amennyiben nem szeretnél a body-ban adatot utaztatni, nem kötelező meghívni.

```csharp
string url = "http://localhost:3000/students"; 
Student student = new Student 
{ 
    phone = "12132", 
    name = "Sanyi", 
    email = "email" 
}; 
Response response = Backend.POST(url).Body(student).Send();
```

---

### `PUT` Kérés Kiépítése

###### Body-val történő Azonosítás

Az objektum tartalmazza az entitás azonosítóját és a módosítani kívánt tulajdonságot az új értékkel együtt. A body felhasználása opcionális: ha nincs szükséged adatok küldésére, a body-t kihagyhatod.

```csharp
string url = "http://localhost:3000/students"; 
Student student = new Student 
{ 
    id = 11, 
    name = "Bela" 
}; 
Response response = Backend.PUT(url).Body(student).Send();
```

###### URL Paraméteres Azonosítás

Az azonosítót az URL tartalmazza, nem szükséges a body-ban megadni. Body felhasználása opcionális. A body felhasználása opcionális: ha nincs szükséged adatok küldésére, a body-t kihagyhatod.

```csharp
string url = "http://localhost:3000/students/11"; 
Student student = new Student { name = "Bela" }; 
Response response = Backend.PUT(url).Body(student).Send();
```

---

### `DELETE` Kérés Kiépítése

###### Body-val történő Azonosítás

Az objektum tartalmazza a törölni kívánt entitás azonosítóját. A body felhasználása opcionális: ha nincs szükséged adatok küldésére, a body-t kihagyhatod.

```csharp
string url = "http://localhost:3000/students"; 
Response response = Backend.DELETE(url)
                           .Body(new Student { id = 11 })
                           .Send();
```

###### URL Paraméteres Azonosítás

Az azonosítót az URL tartalmazza, nem szükséges a body láncolat.

```csharp
string url = "http://localhost:3000/students/11"; 
Response response = Backend.DELETE(url).Send();
```

---

### Adatok Kinyerése a `Response`-ból

###### `As<T>` Metódus

Deszerializálja a kiválasztott JSON adatot a megadott típusra.

```csharp
List<Student> students = Backend.GET(url)
                                .Send()
                                .ValueOf("students")
                                .As<List<Student>>();
```

---

### Részleges Adatkinyerés a `Response`-ból

###### `ValueAt` + `As<T>` Metódus

Kiválaszt egy JSON értéket az adott **index** alapján. Ez a metódus segíti a komplex responseból való részleges adatfeldolgozást.

**Szerver válasz:**

```json
{
    "message": "Dolgozó sikeresen lekérve.",
    "status": "success",
    "data": {
        "Az": 1,
        "Nev": "Nagy József",
        "Telepules": "Szolnok"
    }
}
```

**Részleges adatkinyerés:**

```csharp
Response response = Backend.GET(url).Send(); 
Dolgozo dolgozo= response.ValueAt(2).As<Dolgozo>();
string uzenet= response.ValueAt(0).As<string>();
```

###### `ValueOf` Metódus (OPCIONÁLIS)

Kiválaszt egy JSON értéket a megadott **név** alapján. Ez a metódus segíti a komplex responseból való részleges adatfeldolgozást.

**Szerver válasz:**

```json
{
    "message": "Dolgozó sikeresen lekérve.",
    "status": "success",
    "data": {
        "Az": 1,
        "Nev": "Nagy József",
        "Telepules": "Szolnok"
    }
}
```

**Részleges adatkinyerés:**

```csharp
Response response = Backend.GET(url).Send(); 
Dolgozo dolgozo= response.ValueOf("data").As<Dolgozo>();
string uzenet= response.ValueOf("message").As<string>();
```

---

### Forráskód

A teljes forráskód elérhető itt: [Backend.cs](https://github.com/vellt/Network_Helper_Library/blob/master/NetworkHelper/Backend.cs)

---

### Könyvtár szerkezete

---

Ez a dokumentáció biztosítja, hogy a NetworkHelper könyvtár használata egyszerű és érthető legyen. Ha bármilyen kérdésed van, vagy további segítségre van szükséged, ne habozz kapcsolatba lépni a könyvtár fejlesztőivel vagy a közösséggel a [GitHub Issues](https://github.com/vellt/Network_helper_Library/issues) oldalon.

---

# [‼️ Minta Projektért Kattints Ide ‼️](https://github.com/vellt/minta_projekt_networkhelper)

