const express = require('express');
const cors = require('cors');
const mysql = require('mysql');
const app = express();

const host = 'localhost';
const port = 3000;

app.use(cors());
app.use(express.json());


var connection = mysql.createConnection({
    host: host,
    user: 'root',
    password: '',
    database: 'gyakorlas'
});

app.get('/tanulok/:id', (req, res) => {
    const id = req.params.id;
    connection.query('SELECT * FROM tanulok WHERE id=?', [id], (err, results)=> {
        if(err) {
            console.log(err);
            res.send("hiba");
        }
        else {
            console.log(results);
            res.send(results);
        }
    });
});

app.get('/iskolak', (req, res) => {
    connection.query('SELECT * FROM iskolak', (err, results)=> {
        if(err) {
            console.log(err);
            res.send("hiba");
        }
        else {
            console.log(results);
            res.send(results);
        }
    });
});

app.get('/tanulok', (req, res) => {
    connection.query('SELECT * FROM tanulok', (err, results)=> {
        if(err) {
            console.log(err);
            res.send("hiba");
        }
        else {
            console.log(results);
            res.send(results);
        }
    });
});

app.post('/tanulok', (req, res) => {
    const {nev, email, telefon} = req.body;
    console.log(nev, email, telefon);

    connection.query('INSERT INTO tanulok (id, nev, telefon, email) VALUES (null, ?, ?, ?)', [nev, telefon, email], (err, results)=> {
        if(err) {
            console.log(err);
            res.send("hiba");
        }
        else {
            console.log(results);
            res.send("Sikeres felvétel!");
        }
    });
});


app.put('/tanulok', (req, res) => {
    const {id, nev, email, telefon} = req.body;
    console.log(id, nev, email, telefon);

    let updateColumns = [];
    let updateValues = [];

    if (nev !== undefined) {
        updateColumns.push('nev=?');
        updateValues.push(nev);
    }

    if (email !== undefined) {
        updateColumns.push('email=?');
        updateValues.push(email);
    }

    if (telefon !== undefined) {
        updateColumns.push('telefon=?');
        updateValues.push(telefon);
    }

    const updateQuery = `UPDATE tanulok SET ${updateColumns.join(', ')} WHERE id=?`;

    connection.query(updateQuery,[...updateValues, id], (err, results)=> {
        if (err) {
            console.log(err);
            res.send("hiba");
        } else {
            console.log(results);
            res.send("Sikeres Modosítás!");
        }
    });
});


app.delete('/tanulok', (req, res) => {
    const {id, nev, email, telefon} = req.body;
    console.log(id, nev, email, telefon);

    connection.query('DELETE FROM tanulok WHERE id=?',[id], function (err, results) {
        if (err) {
            console.log(err);
            res.send("hiba");
        } else {
            console.log(results);
            res.send("Sikeres Törlés!");
        }
    });
});

app.listen(port, host,() => {
  console.log(`IP: http://${host}:${port}`);
});
