//FLECHAS UP E DOWN NO MOBILE

function showArrowsOnMobile() {
    if (window.innerWidth <= 800) {
        window.addEventListener('scroll', arrowScrollHandler);
        arrowScrollHandler(); // Atualiza na primeira carga
    } else {
        // Em desktop, garante que nada aparece
        document.querySelectorAll('.up-arrow, .down-arrow').forEach(function (el) {
            el.style.display = 'none';
        });
        window.removeEventListener('scroll', arrowScrollHandler);
    }
}

function arrowScrollHandler() {
    var setaBaixo = document.querySelector('.down-arrow');
    var setaCima = document.querySelector('.up-arrow');
    var scrollTop = window.scrollY || document.documentElement.scrollTop;
    var scrollBottom = (window.innerHeight + scrollTop) >= (document.body.scrollHeight - 10);

    if (scrollTop > 60) {
        setaCima.style.display = "block";
    } else {
        setaCima.style.display = "none";
    }
    if (scrollBottom) {
        setaBaixo.style.display = "none";
    } else {
        setaBaixo.style.display = "block";
    }
}

// Roda na abertura e no resize (caso vire mobile/desktop)
window.addEventListener('load', showArrowsOnMobile);
window.addEventListener('resize', showArrowsOnMobile);