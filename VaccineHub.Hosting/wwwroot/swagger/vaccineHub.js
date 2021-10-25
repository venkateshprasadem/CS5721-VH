(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            // Section 01 - Set url link 
            var logo = document.getElementsByClassName('link');
            logo[0].href = "https://www.ul.ie/";
            logo[0].target = "_blank";
            logo[0].style = "padding: 11px 0 11px 0px;";
            
            // Section 02 - Set logo
            logo[0].children[0].alt = "University of Limerick";
            logo[0].children[0].src = "./resources/logo-with-name.png";
            logo[0].children[0].width = "120";
            logo[0].children[1].textContent = "";

            // Section 03 - Set 32x32 favicon
            const linkIcon32 = document.createElement('link');
            linkIcon32.type = 'image/png';
            linkIcon32.rel = 'icon';
            linkIcon32.href = './resources/favicon-32x32.png';
            linkIcon32.sizes = '32x32';
            document.getElementsByTagName('head')[0].appendChild(linkIcon32);

            // Section 03 - Set 16x16 favicon
            const linkIcon16 = document.createElement('link');
            linkIcon16.type = 'image/png';
            linkIcon16.rel = 'icon';
            linkIcon16.href = './resources/favicon-16x16.png';
            linkIcon16.sizes = '16x16';
            document.getElementsByTagName('head')[0].appendChild(linkIcon16);
        });
    });
})();