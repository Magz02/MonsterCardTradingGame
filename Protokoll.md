# Monster Trading Card Game Protokoll<br>if21b233 - Maksymilian Ormianin<br>WS22 BIF/B2

**Inhaltsverzeichnis**
- [Monster Trading Card Game Protokollif21b233 - Maksymilian OrmianinWS22 BIF/B2](#monster-trading-card-game-protokollif21b233---maksymilian-ormianinws22-bifb2)
  - [1. Design](#1-design)
  - [2. Gewonnene Erkenntnisse](#2-gewonnene-erkenntnisse)
  - [3. Unit Tests](#3-unit-tests)
  - [4. Unique Features](#4-unique-features)
  - [5. Tracked time](#5-tracked-time)
  - [6. GIT Verbindung](#6-git-verbindung)

## 1. Design
Meine Anwendung benutzt in Prinzip zwei Layers: BL und Model Layer. In BL habe ich alle Endpoints definiert die ich benutze um mit der Datenbank sich zu verbinden und dementsprechend Anfragen zu machen. Alle Endpoints habe ich kategorisiert (beispielsweise HTTP oder BM - Battle Management in eigenen Ordner) damit sie leichter zu finden/inkludieren verwalten sind. Model Layer hat alle Modelle für serializen/deserializen also beispielsweise Benutzer oder Package. Alle Modelle werden in MonsterTradingCardGame.Test in Unit Tests durchgetestet. 

## 2. Gewonnene Erkenntnisse
- Dank Projektarbeit konnte ich meine Kenntnisse in C# und .NET entwickeln und vertiefen.
- Ich kann jetzt eine einfache REST-Schnittstelle in C# erstellen.
- Ich kann Unit Tests mit NUnit schreiben.
- Ich kann eine Docker-Container mit postgresql erstellen und dementsprechend verbinden, die Daten verwalten und abfragen.
- Ich kann eine postgresql Datenbank verwenden.

## 3. Unit Tests
Für die Unit Tests habe ich mich für ein set von 20 Tests entschieden, die die Idee von self-sufficiency mithilfe AAA-Pattern anwenden. Die Tests testen die Funktionalität aller in Projekt erstellten Klassen, aber auch die Authentication einer User beim abfragen von Daten in einem Endpoint (wo es nur nötig ist sich zu authentifizieren). Einer von Tests prüft auch ob ein User ist hinzugefügt und entfernt von der Datenbank. Die Tests sind in der Datei `MonsterTradingCardGame.Test/UnitTests.cs` zu finden.

## 4. Unique Features
In meiner Anwendung habe ich ein paar Unique/Bonus Features gesetzt, allerdings präsentiere ich eine als Unique, die reste nur als Bonus Features.

1. Unique feature:
   1. Ein Benutzer bekommt einen +5 zum Damage einer letzten Karte in Deck, wenn er kurz vor verlieren ist.
2. Bonus features:
   1. Elo berechnet nach echtem Elo System. - Dementsprechend ist es auch nicht gut sichtbar dass das Elo-Berechnung funktioniert, weil es würde mehr Gegner brauchen um die Berechunung effizienter zu machen. Es gibt eine vordefinierte floor für die Elo Werte - nämlich Startwert von 100.
   2. Ein Deck muss nicht von einem Benutzer definiert werden, wenn es in der Battle nicht definiert ist, wird es automatisch von 4 besten Karten aus der Datenbank erstellt.
   3. Logout funktionalität - ein Benutzer kann eine Anfrage schicken um sein authoken zu löschen und dementsprechend sich auszuloggen.
   4. Methode die schaut ob ein Benutzer Karten hat bevor man den Benutzer als Gegner in eine Battle einfügt. Ist der Benutzer nicht in der Lage eine Karte zu haben, wird er nicht in die Battle eingefügt, und es wird nach nächsten Benutzer gesucht.

## 5. Tracked time
Für die Anwendung habe ich grundsätzlich ca. 10 Stunden wöchentlich ab November gebraucht, das bringt auf Gesamtaufwand von 100 Stunden. Für die Dokumentation selbst habe ich ca. 1h 30min gebraucht. Die Längste Zeit habe ich für die Battle-Logik und Funktionalitäten gebraucht - die ist auch allerdings am komplexesten von allen Endpoints. Da würde die Schätzung von Zeit mich auf 10 Stunden bringen, da ich 1 Woche dafür gebraucht habe. Am kürzesten war die Zeit für die Unit-Tests, da ich nur 1 Tag dafür gebraucht habe.

## 6. GIT Verbindung
Alle Commits sind in meinem GitHub Repository zu finden:
https://github.com/TORz69/MonsterCardTradingGame.git