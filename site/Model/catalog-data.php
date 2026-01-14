<?php
// ---------- CONFIG ----------
// Importa o arquivo de conexão com o banco
require 'Conexao.php';

// Monta a consulta com possibilidade de filtro por nome via GET
$sql = "SELECT Cat_id, Cat_nome, Cat_desc, Cat_cate, Cat_tamanho, Cat_img1, Cat_img2, Cat_img3, Cat_img4 FROM Catalogo WHERE 1=1";

$params = [];
$types = "";

// Filtro por nome
if (!empty($_GET['name'])) {
    $sql .= " AND Cat_nome LIKE ?";
    $params[] = "%" . $_GET['name'] . "%";
    $types .= "s";
}

// Filtro por categorias (array)
if (!empty($_GET['categoria']) && is_array($_GET['categoria'])) {
    $placeholders = implode(',', array_fill(0, count($_GET['categoria']), '?'));
    $sql .= " AND Cat_cate IN ($placeholders)";
    foreach ($_GET['categoria'] as $cat) {
        $params[] = $cat;
        $types .= "s";
    }
}

// Filtro por tamanho (array)
if (!empty($_GET['tamanho']) && is_array($_GET['tamanho'])) {
    $placeholders = implode(',', array_fill(0, count($_GET['tamanho']), '?'));
    $sql .= " AND Cat_tamanho IN ($placeholders)";
    foreach ($_GET['tamanho'] as $tam) {
        $params[] = $tam;
        $types .= "s";
    }
}

// Preparar e executar a query
$stmt = $conn->prepare($sql);

if (!empty($params)) {
    $stmt->bind_param($types, ...$params);
}

$stmt->execute();
$result = $stmt->get_result();

$catalogItems = [];
while ($row = $result->fetch_assoc()) {
    $catalogItems[] = $row;
}

// Obter categorias dinâmicas para filtro no template
// $sqlCat = "SELECT DISTINCT Cat_cate FROM Catalogo";
// $resultCat = $conn->query($sqlCat);
// $categorias = [];
// while ($row = $resultCat->fetch_assoc()) {
//     $categorias[] = $row['Cat_cate'];
// }
$sqlCat = "
    SELECT DISTINCT Cat_cate
    FROM Catalogo
    ORDER BY 
        CASE 
            WHEN Cat_cate REGEXP '^[0-9]+' THEN 0  -- primeiro nomes que começam com número
            ELSE 1                               -- depois apenas letras
        END,
        Cat_cate ASC                             -- ordem alfabética
";
$resultCat = $conn->query($sqlCat);
$categorias = [];
while ($row = $resultCat->fetch_assoc()) {
    $categorias[] = $row['Cat_cate'];
}


// Obter tamanhos dinâmicos para filtro no template
// $sqlTam = "SELECT DISTINCT Cat_tamanho FROM Catalogo";
$sqlTam = "
    SELECT DISTINCT Cat_tamanho 
    FROM Catalogo
    ORDER BY 
        CASE 
            WHEN Cat_tamanho REGEXP '^[0-9]+' THEN 0  -- primeiro tamanhos numéricos (tipo 1, 2, 10)
            ELSE 1                                   -- depois textos (Pequeno, Médio, Grande)
        END,
        CAST(Cat_tamanho AS UNSIGNED),              -- ordena números 1,2,10 corretamente
        Cat_tamanho ASC                             -- e por fim ordena nomes em ordem alfabética
";
$resultTam = $conn->query($sqlTam);
$tamanhos = [];
while ($row = $resultTam->fetch_assoc()) {
    $tamanhos[] = $row['Cat_tamanho'];
}

$conn->close();
?>