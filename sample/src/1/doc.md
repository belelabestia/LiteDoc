<style>
    @page {
        @top-center {
            content: element(header)
        }

        @bottom-center {
            content: element(footer);
        }
    }

    .header {
        position: running(header);
    }

    .footer {
        position: running(footer);
    }

    .yellow-bg {
        color: yellow;
    }

    .blue-bg {
        color: blue;
    }
</style>

<div class="header blue-bg">Doc 1 Header</div>
<div class="footer yellow-bg">Doc 1 Footer</div>

# Doc 1

Doc 1 works!

## Titolone

{text:replaceThis}