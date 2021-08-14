# LiteDoc

LiteDoc trasforma la tua documentazione HTML e Markdown in un unico PDF pronto per la stampa.

## Funzionalità

Crea uno _spazio di lavoro_, aggiusta la configurazione e inizia subito a lavorare sui tuoi documenti.

LiteDoc supporta la creazione di più sezioni di diversi formati e supporta nativamente le regole CSS dedicate alla stampa, grazie a Weasyprint.

LiteDoc può rimanere in ascolto su una cartella e aggiornare automaticamente il risultato definitivo dei tuoi documenti.

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

Il progetto non è ancora completo, questa è l'agenda che ho pensato per ora:

- [ ] Usare .NET Generic Host per avere Dependency Injection nativa e miglior supporto per l'application lifetime.
- [ ] Trovare un sistema efficace per aprire e riavviare il browser ogni volta che l'output viene aggiornato.
- [ ] Realizzare la creazione di un nuovo progetto.
- [ ] Aggiungere margine di configurabilità, tramite i parametri e/o tramite configurazione.
