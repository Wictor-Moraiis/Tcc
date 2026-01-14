<!doctype html>
<html lang="pt-br">

<head>
    <title>Marcenaria Morais</title>
    <link rel="icon" type="image/x-icon" href="./img/white_logo.png">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link href="./Catalog.css" rel="stylesheet" />
    <link rel="stylesheet"
        href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0&icon_names=search" />
</head>

<body>
    <!-- NAVBAR -->
    <nav>
        <a class="nav-logo" href="../../View/Home/home.html"><img src="./img/white_logo.png"
                alt="Logo Marcenaria Morais"></a>
        <a class="nav-link" href="../../View/Home/home.html">HOME</a>
    </nav>

    <main>
        <!-- FILTERS -->
        <section id="filters">
            <form method="get" action="">

                <!-- SEARCH -->
                <div class="filters-div">
                    <div class="search">
                        <input type="search" id="name" name="name" placeholder="pesquisar"
                            value="<?php echo isset($_GET['name']) ? htmlspecialchars($_GET['name']) : ''; ?>">
                        <button type="submit"><img src="./img/icon_white_search.png"></button>
                    </div>
                </div>

                <!-- SIZE -->
                <div class="filters-div">
                    <p>Tamanho do produto:</p>

                    <div class="size">
                        <?php foreach($tamanhos as $index => $tamanho): ?>
                        <div>
                            <input type="checkbox" name="tamanho[]" id="tam_<?php echo $index; ?>"
                                value="<?php echo htmlspecialchars($tamanho); ?>"
                                <?php if(isset($_GET['tamanho']) && in_array($tamanho, $_GET['tamanho'])) echo "checked"; ?>>
                            <label for="tam_<?php echo $index; ?>"><?php echo htmlspecialchars($tamanho); ?></label>
                        </div>
                        <?php endforeach; ?>
                    </div>
                </div>


                <!-- CATEGORY -->
                <div class="filters-div">
                    <p>Categoria:</p>
                    <div class="category">
                        <?php foreach($categorias as $index => $cat): ?>
                        <label>
                            <input type="checkbox" name="categoria[]" id="cat_<?php echo $index; ?>"
                                value="<?php echo htmlspecialchars($cat); ?>" <?php
                    if(isset($_GET['categoria']) && in_array($cat, $_GET['categoria']))
                        echo "checked";
                ?>>
                            <span><?php echo htmlspecialchars($cat); ?></span>
                        </label>
                        <?php endforeach; ?>
                    </div>
                </div>

                <button type="submit" style="display: none;">Filtrar</button>

            </form>
        </section>

        <!-- CATALOG -->
        <section id="catalog">
            <div class="catalog">
                <?php if (count($catalogItems) > 0): ?>
                <?php foreach ($catalogItems as $item): ?>
                <div class="card" onclick="showPopup('<?php echo $item['Cat_id']; ?>')">
                    <img class="card-img" src="./img/catalog/<?php echo htmlspecialchars($item['Cat_img1']); ?>"
                        alt="Imagem de <?php echo htmlspecialchars($item['Cat_nome']); ?>">
                    <div class="card-txt">
                        <h5><?php echo htmlspecialchars($item['Cat_nome']); ?></h5>
                        <p><?php echo htmlspecialchars(substr($item['Cat_desc'], 0, 80)) . '...'; ?></p>
                        <a href="#">Clique para mais informações</a>
                    </div>
                </div>

                <!-- POPUP -->
                <section class="popup-content" id="popup-<?php echo $item['Cat_id']; ?>" style="display:none;">
                    <div class="popup">
                        <button onclick="hidePopup('<?php echo $item['Cat_id']; ?>')"><img
                                src="./img/icon_dark_close.png" alt="Fechar popup"></button>
                        <div class="popup-columns">
                            <div class="popup-left">
                                <!-- Container do carrossel -->
                                <div class="carousel-container" id="carousel-<?php echo $item['Cat_id']; ?>">
                                    <div class="carousel-track">
                                        <?php for ($i=1; $i<=4; $i++): ?>
                                        <?php if (!empty($item["Cat_img$i"])): ?>
                                        <div class="carousel-slide">
                                            <img src="./img/catalog/<?php echo htmlspecialchars($item["Cat_img$i"]); ?>"
                                                alt="Imagem adicional de <?php echo htmlspecialchars($item['Cat_nome']); ?>">
                                        </div>
                                        <?php endif; ?>
                                        <?php endfor; ?>
                                    </div>

                                    <!-- Botões de navegação -->
                                    <button class="carousel-btn carousel-prev"
                                        onclick="moveCarousel('<?php echo $item['Cat_id']; ?>', -1)">
                                        <img src="./img/icon_white_left_arrow.png" alt="Anterior">
                                    </button>
                                    <button class="carousel-btn carousel-next"
                                        onclick="moveCarousel('<?php echo $item['Cat_id']; ?>', 1)">
                                        <img src="./img/icon_white_right_arrow.png" alt="Próximo">
                                    </button>
                                </div>
                            </div>
                            <div class="popup-right">
                                <h5><?php echo htmlspecialchars($item['Cat_nome']); ?></h5>
                                <p><?php echo htmlspecialchars($item['Cat_desc']); ?></p><br>
                                <p>Tamanho</p>
                                <span><?php echo htmlspecialchars($item['Cat_tamanho']); ?></span>
                                <p>Categorias</p>
                                <span><?php echo htmlspecialchars($item['Cat_cate']); ?></span>
                            </div>
                        </div>
                    </div>
                </section>
                <?php endforeach; ?>
                <?php else: ?>
                <p>Nenhum item encontrado.</p>
                <?php endif; ?>
            </div>
        </section>

        <!-- PAGES -->
        <section id="pages">
            <div class="pages"></div>
        </section>
    </main>

    <footer>
        <p>©2025 <a href="../Devs/devs.html">Desenvolvedores</a></p>
    </footer>
    <script src="../../Controller/Catalog.js"></script>
</body>

</html>