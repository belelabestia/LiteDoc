# LiteDoc

LiteDoc trasforma la tua documentazione HTML e Markdown in un unico PDF pronto per la stampa.

---

- Crea uno _spazio di lavoro_, aggiusta la configurazione e inizia subito a lavorare sui tuoi documenti.
- LiteDoc supporta la creazione di più sezioni di diversi formati e supporta nativamente le regole CSS dedicate alla stampa, grazie a Weasyprint.
- LiteDoc può rimanere in ascolto su una cartella e aggiornare automaticamente il risultato definitivo dei tuoi documenti.

## Uso

> Per compilare e installare LiteDoc è necessario `.NET SDK 5.0`.  
> Per installarlo, andare [qui](https://dotnet.microsoft.com/download/visual-studio-sdks) e scaricare la versione 5 per la propria piattaforma.

Gli script disponibili nel progetto installano litedoc come un `dotnet tool` globale a partire dal progetto; per ora, LiteDoc non è pubblicato su nuget o altre sorgenti.

Per installare: **dalla cartella del progetto**, lanciare:

```
.\scripts\install.ps1
```

Per aggiornare, **sempre dalla cartella del progetto**, lanciare:

```
.\scripts\update.ps1
```

> Gli script non contengono comandi powershell e possono essere copia-incollati anche in bash

## Formati supportati

Attualmente LiteDoc supporta i seguenti formati:

- HTML
- Markdown

Il formato di output è PDF.

## OSS di terze parti

LiteDoc esiste grazie a:

- PdfSharp
- Weasyprint
- Markdig

## Tabella di marcia

Ho tante idee su come portare avanti LiteDoc.  
Principalmente, vorrei lavorare molto sulla sua espandibilità.

In particolare:

- vorrei idealmente supportare un numero indefinito di formati
- vorrei facilitare la creazione di middleware che manipolino i documenti prima di generare il risultato finale

Un'idea più chiara e aggiornata dei miei piani sarà disponibile [qui](https://github.com/belelabestia/LiteDoc/projects).
