<?php

// Configurações do banco de dados remoto
$host     = "SEU_HOST_AQUI";
// $host     = "162.241.2.71";
$usuario  = "SEU_USER_AQUI";
$senha    = "SUA_SENHA_AQUI";
$banco    = "SEU_BANCO_AQUI";

// Criar conexão usando mysqli
$conn = new mysqli($host, $usuario, $senha, $banco);
$conn->set_charset("utf8mb4");

?>
