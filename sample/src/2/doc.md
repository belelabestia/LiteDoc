<style>
    @page {
        @top-center {
            content: element(header);
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

<div class="header blue-bg">Doc 2 Header</div>
<div class="footer yellow-bg">Doc 2 Footer</div>

# Doc 2

Doc 2 works!