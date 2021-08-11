# LiteDoc

Quella che doveva essere una semplice web app ora diventa uno strumento da riga di comando.

## Note

Sono riuscito a prendere la configurazione, recuperare i documenti, parsare ciò che è da parsare, creare e concatenare i PDF.

L'obiettivo attuale è strutturare una buona architettura da poi estendere.

### Gestione errori

Ho creato un tipo IOption e un'interfaccia IValidatable per gestire validazioni e possibili errori; andrò avanti ad espanderlo man mano che ne riscontro l'utilità.

Sicuramente anche questa parte può essere migliorata. Non mi piace tanto l'approccio con la classe statica che implementa le funzioni e poi le estensioni che ne riprendono i metodi; avere una sola implementazione (gli extension method mi piacciono di più) sarebbe decisamente meglio.

### Trasformazione delle sezioni

Prima di generare i PDF delle sezioni, devo prevedere la possibilità di inserire delle trasformazioni per le stesse, con eventualmente delle parti di configurazione dedicate.

Qui ci sarà da studiare un pochino come fare.

## Pipeline

```
Configuration.SectionConfigurations => ForEach => (
    (Content.SectionContent, Format.SectionParser) =>
    Parser.SectionParsed =>
    HtmlWriter.SectionHtmlWritten =>
    PdfWriter.SectionPdfWritten =>
) => PdfConcat.DocumentPdfWritten;
```

## Funzionalità

- _LiteDoc_ lavora in una cartella e usa un file di configurazione per recuperare i percorsi del progetto.
- **Prende** la configurazione e tutti i file.
- **Rende** un pdf con tutti i documenti concatenati e stilizzati.
- _LiteDoc_ può anche essere utilizzato per generare una nuova cartella di progetto.
- **Usa** weasyprint o wkhtmltopdf per generare i pdf