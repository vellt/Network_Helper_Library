# NetworkHelper.dll

- **A NetworkHelper** egy .NET Framework és C# projektekhez készült könyvtár, amely megkönnyíti a backend kommunikációt. A könyvtár egyszerűsíti az HTTP kérések küldését és a JSON válaszok feldolgozását.
- **Verzió**: v0.1.3
- **Támogatott .NET Verziók**: .NET Framework 4.7.2 vagy újabb

### Első lépések

- **Könyvtár (DLL) letöltése**:
  
  - Töltsd le a legújabb `NetworkHelper.dll` fájlt a [GitHub Releases](https://github.com/vellt/Network_helper_Library/releases) oldalról.

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

Body felhasználása opcionális. Amennyiben nem szeretnél a body-ban adatot utaztatni, nem kötelező meghívni.

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

A body felhasználása opcionális: ha nincs szükséged adatok küldésére, a body-t kihagyhatod.

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

A body felhasználása opcionális: ha nincs szükséged adatok küldésére, a body-t kihagyhatod.

```csharp
string url = "http://localhost:3000/students/11"; 
Student student = new Student { name = "Bela" }; 
Response response = Backend.PUT(url).Body(student).Send();
```

---

### `DELETE` Kérés Kiépítése

###### Body-val történő Azonosítás

A body felhasználása opcionális: ha nincs szükséged adatok küldésére, a body-t kihagyhatod.

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

### `Teljes` adatkinyerés a `Response`-ból

Deszerializálja a kiválasztott JSON adatot a megadott típusra.

**Server response:**
```json
[
    {
        "id": 1,
        "nev": "Kiss Péter",
        "osztaly": "9.A",
        "szuletesi_datum": "2007-05-12"
    },
    {
        "id": 2,
        "nev": "Nagy Anna",
        "osztaly": "10.B",
        "szuletesi_datum": "2006-08-23"
    }
]
```

**`As<T>` Metódus:**

```csharp
List<Student> students = Backend.GET(url).Send().As<List<Student>>();
```

---

### `Részleges` adatkinyerés a `Response`-ból

Ezen metódusok segítik a komplex responseból való részleges adatfeldolgozást.

**Komplex server response:**

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

**`ValueAt` + `As<T>` Metódus:** A `ValueAt` kiválaszt egy JSON értéket az adott **index** alapján. 

```csharp
Response response = Backend.GET(url).Send();
string uzenet= response.ValueAt(0).As<string>();
string status= response.ValueAt(1).As<string>();
Dolgozo dolgozo= response.ValueAt(2).As<Dolgozo>();
```

**`ValueOf` + `As<T>` Metódus:** A `ValueOf` kiválaszt egy JSON értéket a megadott **név** alapján.

```csharp
Response response = Backend.GET(url).Send();
string uzenet= response.ValueOf("message").As<string>();
string status= response.ValueOf("status").As<string>();
Dolgozo dolgozo= response.ValueOf("data").As<Dolgozo>();
```

---

### `ValueAt`- és `ValueOf`-ok egymásba ágyazása

Ezen metódusok egymásba ágyazása segíti a komplex responseból a mélyebb szintű részleges adatfeldolgozást. Akár kombinálhatjuk is őket (`ValueAt`-ra `ValueOf` vagy fordítva). Ezáltal csökkenteni tudjuk a deszerializálandó adatmennyiséget, így **erőforrást szabadíthatunk fel** projektünkben.

**Komplex server response:**

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

**A `Nev` érték kinyerése a `ValueAt`-al** 

```csharp
string nev = Backend.GET(link).Send()
                .ValueAt(2)
                .ValueAt(1)
                .As<string>();
```

**A `Nev` érték kinyerése a `ValueOf`-al** 

```csharp
string nev = Backend.GET(link).Send()
                .ValueOf("data")
                .ValueOf("Nev")
                .As<string>();
```

**A `Nev` érték kinyerése a `ValueAt` és `ValueOf`-al** 

```csharp
string nev = Backend.GET(link).Send()
                .ValueAt(2)
                .ValueOf("Nev")
                .As<string>();
```

---

### Fájlfeltöltés

```csharp
Backend.UPLOAD(url).File(filePath).Send();
```

**Szerver válasz formátuma**: sikeres feltöltéskor visszatér a fájl nevével, a data tulajdonságba helyezve.
```json
{
    "message": "Fájl sikeresen feltöltve.",
    "status": "success",
    "data": "3126503.png"
}
```

**Fájl feltöltése, válasz megjelenítése**

Fájl útvonálának kiválasztásához érdemes a OpenFileDialog-ot használni WPF esetében. A kódrészletben kiválasztom a képet, és útvonalát eltárolom a filePath változóban.

```csharp
OpenFileDialog openFileDialog = new OpenFileDialog
{
    Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*"
};

if (openFileDialog.ShowDialog() == true)
{
    string filePath = openFileDialog.FileName;

    string url = "http://localhost:3000/upload";
    Response response = Backend.UPLOAD(url).File(filePath).Send();

    // megjelenítem a válaszüzenetet
    MessageBox.Show(response.ValueOf("message").As<string>());
}
```

---

### Forráskód

A teljes forráskód elérhető itt: [Backend.cs](https://github.com/vellt/Network_Helper_Library/blob/master/NetworkHelper/Backend.cs)

---

### Könyvtár szerkezete

<img width="878" alt="ClassDiagram1" src="https://github.com/user-attachments/assets/537d101e-9878-4cfe-97d7-571af40beee8">

---

Ez a dokumentáció biztosítja, hogy a NetworkHelper könyvtár használata egyszerű és érthető legyen. Ha bármilyen kérdésed van, vagy további segítségre van szükséged, ne habozz kapcsolatba lépni velem a [GitHub Issues](https://github.com/vellt/Network_helper_Library/issues) oldalon.

---

# [‼️ Minta Projektért Kattints Ide ‼️](https://github.com/vellt/minta_projekt_networkhelper)

