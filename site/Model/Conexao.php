<?php

// Configurações do banco de dados remoto
$host     = "br910.hostgator.com.br";
// $host     = "162.241.2.71";
$usuario  = "quaiat07_marcenaria_morais";
$senha    = "21314151**mm";
$banco    = "quaiat07_marcenaria_morais";

// Criar conexão usando mysqli
$conn = new mysqli($host, $usuario, $senha, $banco);
$conn->set_charset("utf8mb4");

?>