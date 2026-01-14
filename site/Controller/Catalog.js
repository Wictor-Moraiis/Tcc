//POPUP
function showPopup(id) {
    const popup = document.getElementById('popup-' + id);
    popup.style.display = 'flex';
    // Inicializa o carrossel quando o popup abre
    setTimeout(() => initCarousel(id), 50);
}

function hidePopup(id) {
    document.getElementById('popup-' + id).style.display = 'none';
}

// Filtros dinâmicos sem botão
document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    const checkboxes = form.querySelectorAll("input[type='checkbox']");

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", function () {
            form.submit();
        });
    });
});

// Navegação por teclado para o carrossel
document.addEventListener('keydown', function (e) {
    const openPopup = document.querySelector('.popup-content[style*="display: flex"]');
    if (!openPopup) return;

    const popupId = openPopup.id.replace('popup-', '');

    if (e.key === 'ArrowLeft') {
        moveCarousel(popupId, -1);
    } else if (e.key === 'ArrowRight') {
        moveCarousel(popupId, 1);
    } else if (e.key === 'Escape') {
        hidePopup(popupId);
    }
});

// CARROSSEL
function initCarousel(popupId) {
    const container = document.querySelector(`#popup-${popupId} .carousel-container`);
    if (!container) return;

    const track = container.querySelector('.carousel-track');
    const prevBtn = container.querySelector('.carousel-prev');
    const nextBtn = container.querySelector('.carousel-next');

    // Garante que estamos no primeiro slide
    track.scrollLeft = 0;

    // Atualiza estado inicial dos botões
    updateCarouselButtons(popupId);
}

function moveCarousel(popupId, direction) {
    const container = document.querySelector(`#popup-${popupId} .carousel-container`);
    if (!container) return;

    const track = container.querySelector('.carousel-track');
    const slides = container.querySelectorAll('.carousel-slide');

    if (!slides.length) return;

    const slideWidth = track.offsetWidth;
    const currentPosition = track.scrollLeft;
    const maxScroll = track.scrollWidth - track.offsetWidth;

    let targetPosition;

    if (direction === 1) {
        // Próximo
        targetPosition = Math.min(currentPosition + slideWidth, maxScroll);
    } else {
        // Anterior
        targetPosition = Math.max(currentPosition - slideWidth, 0);
    }

    track.scrollTo({
        left: targetPosition,
        behavior: 'smooth'
    });

    // Atualiza botões após a animação
    setTimeout(() => updateCarouselButtons(popupId), 300);
}

function updateCarouselButtons(popupId) {
    const container = document.querySelector(`#popup-${popupId} .carousel-container`);
    if (!container) return;

    const track = container.querySelector('.carousel-track');
    const prevBtn = container.querySelector('.carousel-prev');
    const nextBtn = container.querySelector('.carousel-next');

    const currentPosition = track.scrollLeft;
    const maxScroll = track.scrollWidth - track.offsetWidth;

    // Adiciona uma tolerância para evitar problemas de arredondamento
    const tolerance = 1;

    prevBtn.disabled = currentPosition <= tolerance;
    nextBtn.disabled = currentPosition >= maxScroll - tolerance;
}

// PAGINAÇÃO AUTOMÁTICA
document.addEventListener("DOMContentLoaded", function () {
    const cards = document.querySelectorAll(".card");
    const pagesContainer = document.querySelector(".pages");
    const itensPorPagina = 24;
    let paginaAtual = 1;
    const totalPaginas = Math.ceil(cards.length / itensPorPagina);

    function mostrarPagina(pagina) {
        const inicio = (pagina - 1) * itensPorPagina;
        const fim = inicio + itensPorPagina;

        cards.forEach((card, index) => {
            if (index >= inicio && index < fim) {
                card.style.display = "block";
            } else {
                card.style.display = "none";
            }
        });

        // volta aos filtros
        const filtros = document.getElementById('filters');
        if (filtros) {
            filtros.scrollIntoView({ behavior: "smooth" });
        }

        atualizarBotoes();
    }

    function atualizarBotoes() {
        pagesContainer.innerHTML = "";

        // Botão Anterior
        const btnAntes = document.createElement("button");
        btnAntes.textContent = "Anterior";
        btnAntes.classList.add("btn-before");
        btnAntes.disabled = paginaAtual === 1;
        btnAntes.onclick = () => {
            if (paginaAtual > 1) {
                paginaAtual--;
                mostrarPagina(paginaAtual);
            }
        };
        pagesContainer.appendChild(btnAntes);

        // Botões Numéricos
        for (let i = 1; i <= totalPaginas; i++) {
            const btn = document.createElement("button");
            btn.textContent = i;
            btn.classList.add("btn-number");
            if (i === paginaAtual) btn.classList.add("active");
            btn.onclick = () => {
                paginaAtual = i;
                mostrarPagina(paginaAtual);
            };
            pagesContainer.appendChild(btn);
        }

        // Botão Próximo
        const btnDepois = document.createElement("button");
        btnDepois.textContent = "Próximo";
        btnDepois.classList.add("btn-after");
        btnDepois.disabled = paginaAtual === totalPaginas;
        btnDepois.onclick = () => {
            if (paginaAtual < totalPaginas) {
                paginaAtual++;
                mostrarPagina(paginaAtual);
            }
        };
        pagesContainer.appendChild(btnDepois);
    }

    // INICIALIZAÇÃO
    mostrarPagina(paginaAtual);
});