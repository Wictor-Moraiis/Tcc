-- --------------------------------------------------------
-- Servidor:                     127.0.0.1
-- Versão do servidor:           8.0.30 - MySQL Community Server - GPL
-- OS do Servidor:               Win64
-- HeidiSQL Versão:              12.1.0.6537
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Copiando estrutura do banco de dados para marcenaria_morais
CREATE DATABASE IF NOT EXISTS `marcenaria_morais` /*!40100 DEFAULT CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `marcenaria_morais`;

-- Copiando estrutura para tabela marcenaria_morais.catalogo
CREATE TABLE IF NOT EXISTS `catalogo` (
  `Cat_id` int NOT NULL AUTO_INCREMENT,
  `Cat_img1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cat_img2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Cat_img3` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Cat_img4` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Cat_nome` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cat_cate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cat_tamanho` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cat_desc` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  PRIMARY KEY (`Cat_id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela marcenaria_morais.categoria_funcionario
CREATE TABLE IF NOT EXISTS `categoria_funcionario` (
  `Catg_id` int NOT NULL AUTO_INCREMENT,
  `Catg_nome` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Catg_sal` double(10,2) NOT NULL,
  PRIMARY KEY (`Catg_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela marcenaria_morais.cliente
CREATE TABLE IF NOT EXISTS `cliente` (
  `Cli_id` int NOT NULL AUTO_INCREMENT,
  `Cli_cpf` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_cpf_hash` char(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_nome` varchar(512) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_tel1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_tel1_hash` char(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_tel2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Cli_cep` int NOT NULL,
  `Cli_bairro` varchar(512) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_rua` varchar(512) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_num_casa` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Cli_complemento` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Cli_id`),
  UNIQUE KEY `Cli_cpf_hash` (`Cli_cpf_hash`),
  UNIQUE KEY `Cli_tel1_hash` (`Cli_tel1_hash`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela marcenaria_morais.estoque
CREATE TABLE IF NOT EXISTS `estoque` (
  `Estq_id` int NOT NULL AUTO_INCREMENT,
  `Estq_produto` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Estq_quant` int NOT NULL,
  `Estq_tel_forne` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Estq_img` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Estq_id`),
  UNIQUE KEY `Estq_produto` (`Estq_produto`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela marcenaria_morais.financeiro
CREATE TABLE IF NOT EXISTS `financeiro` (
  `Fin_id` int NOT NULL AUTO_INCREMENT,
  `Fin_mov_desc` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Fin_mov_val` float(10,2) NOT NULL,
  `Fin_data` date NOT NULL,
  PRIMARY KEY (`Fin_id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela marcenaria_morais.funcionario
CREATE TABLE IF NOT EXISTS `funcionario` (
  `Func_id` int NOT NULL AUTO_INCREMENT,
  `Func_cpf` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Func_cpf_hash` char(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Func_nome` varchar(512) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Func_tel1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Func_tel1_hash` char(64) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Func_tel2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Func_desc` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `Catg_id` int NOT NULL,
  PRIMARY KEY (`Func_id`),
  UNIQUE KEY `Func_cpf_hash` (`Func_cpf_hash`),
  UNIQUE KEY `Func_tel1_hash` (`Func_tel1_hash`),
  KEY `Catg_id` (`Catg_id`),
  CONSTRAINT `Funcionario_ibfk_1` FOREIGN KEY (`Catg_id`) REFERENCES `categoria_funcionario` (`Catg_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela marcenaria_morais.pedido
CREATE TABLE IF NOT EXISTS `pedido` (
  `Ped_id` int NOT NULL AUTO_INCREMENT,
  `Ped_realizado` date NOT NULL,
  `Ped_entrega` date NOT NULL,
  `Ped_executado` tinyint(1) NOT NULL,
  `Ped_desc` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `Ped_valor` float(10,2) NOT NULL,
  `Cli_id` int NOT NULL,
  `Estq_id1` int DEFAULT NULL,
  `Estq_id2` int DEFAULT NULL,
  `Estq_id3` int DEFAULT NULL,
  `Estq_id4` int DEFAULT NULL,
  `Estq_id5` int DEFAULT NULL,
  PRIMARY KEY (`Ped_id`),
  KEY `Cli_id` (`Cli_id`),
  KEY `Estq_id1` (`Estq_id1`),
  KEY `Estq_id2` (`Estq_id2`),
  KEY `Estq_id3` (`Estq_id3`),
  KEY `Estq_id4` (`Estq_id4`),
  KEY `Estq_id5` (`Estq_id5`),
  CONSTRAINT `Pedido_ibfk_1` FOREIGN KEY (`Cli_id`) REFERENCES `cliente` (`Cli_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Pedido_ibfk_2` FOREIGN KEY (`Estq_id1`) REFERENCES `estoque` (`Estq_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Pedido_ibfk_3` FOREIGN KEY (`Estq_id2`) REFERENCES `estoque` (`Estq_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Pedido_ibfk_4` FOREIGN KEY (`Estq_id3`) REFERENCES `estoque` (`Estq_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Pedido_ibfk_5` FOREIGN KEY (`Estq_id4`) REFERENCES `estoque` (`Estq_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Pedido_ibfk_6` FOREIGN KEY (`Estq_id5`) REFERENCES `estoque` (`Estq_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Exportação de dados foi desmarcado.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
