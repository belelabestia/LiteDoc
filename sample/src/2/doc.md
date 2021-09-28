<style>
    @top-center {
        content: element(header);
    }

    @bottom-center {
        content: element(footer);
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

<div class="header blue-bg">This is</div>
<div class="footer yellow-bg">Doc 2 Footer</div>

# Doc 2

Doc 2 works!

## {text:replaceThis}

{text:andThis}