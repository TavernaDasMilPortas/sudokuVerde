# Game Design Document (GDD) — Sudoku Verde

## 1. Conceito do Jogo
**Sudoku Verde** é um jogo de lógica leve e relaxante com temática de jardinagem, desenvolvido em Unity (UI Toolkit) para dispositivos móveis em orientação retrato (`400 × 800`, proporção `1:2`). O jogador posiciona uma planta por região de cor, linha e coluna, sem que as plantas se toquem.

## 2. Público-Alvo
- **Principal:** Público **60+** (idosos e adultos maduros), habituados ou em adaptação a telas sensíveis ao toque.
- **Necessidades consideradas:**
  - Alto contraste entre texto e fundo.
  - Botões largos e alvos táteis generosos (`≥ 52 px` de altura/largura na referência `400 × 800`).
  - Tipografia limpa, sem serifa e legível.
  - Fluxo direto com baixa carga cognitiva e feedback claro das ações.

## 3. Objetivos
- Oferecer exercício cognitivo acessível (raciocínio lógico e atenção espacial) em partidas curtas.
- Demonstrar duas propostas funcionais de interface em Unity UI Toolkit conectadas por Data Binding em tempo de execução para realização de Teste A/B.

## 4. Regras do Jogo
Cada tabuleiro `4 × 4` possui 4 regiões coloridas contínuas e exige exatamente 4 plantas posicionadas seguindo três regras:
1. **1 planta por cor:** Cada região colorida contém exatamente uma planta.
2. **1 planta por coluna e linha:** Nenhuma linha ou coluna pode ter mais de uma planta.
3. **As plantas não podem se tocar:** Duas plantas não podem ocupar células vizinhas ortogonal ou diagonalmente.

## 5. Mecânicas
- **Tabuleiro interativo (`4 × 4`):** Células independentes com três estados (`Vazia`, `Marcada com X`, `Com Planta`).
- **Alternância de ferramenta:** Botão de ação alterna entre colocação de **Planta** e marcação auxiliar **X** (para descartar casas impossíveis).
- **Sistema de Dicas:** Limite por fase (`3` dicas) que revela uma posição correta ou corrige um conflito existente.
- **Validação em tempo real:** Destaque visual imediato quando há conflito entre plantas; detecção automática de vitória quando as 4 plantas válidas são posicionadas.
- **Desafio Diário:** Seleciona um tabuleiro a partir da data atual e incrementa a sequência diária ao ser concluído.

## 6. Controles
- **Toque único (tap):** Selecionar botões de menu, alternar ferramenta, acionar dica e alternar o estado das células do tabuleiro.

## 7. Loop de Gameplay
1. Jogador inicia no **Menu Inicial** e escolhe **JOGAR** ou **DESAFIO DIARIO**.
2. Na tela de **Gameplay**, alterna entre colocar **Planta** e marcar **X**, podendo consultar **Como Jogar** (`?`) ou pausar (`Voltar`).
3. Ao cumprir as 3 regras com todas as plantas no tabuleiro, abre automaticamente a **END Screen (Fase Concluída!)**.
4. Jogador escolhe **PROXIMA FASE** (avança nível) ou **VOLTAR AO MENU** (com sequência diária/progresso atualizados).

## 8. Direção Artística
- **Estilo:** Ilustração manual acolhedora com bordas orgânicas arredondadas e temática botânica.
- **Paleta principal:**
  - Verde gramado (`#AFD46E`), verde borda (`#69B34C`), verde título (`#367B28`).
  - Creme claro dos painéis/botões (`#EEF6C5`), texto escuro (`#231F20`).
  - Regiões do tabuleiro: Laranja, Azul, Verde e Verde-claro.
  - Emblema de contagem: Vermelho de alto destaque (`badge_base`).
- **Tipografia:** Referência original utiliza `Winkle-Regular`. Na ausência da fonte licenciada no repositório, foi utilizada `Lato-Bold` como substituta legível.

## 9. Interfaces (Escopo UI Toolkit — Variações A e B)
O projeto apresenta duas variações funcionais de interface conectadas aos mesmos ViewModels, regras e tabuleiros, alternáveis em tempo real pelo seletor **Versão da interface** em Configurações:
- **Variação A:** Menu com sequência diária e Desafio Diário; HUD com alternador explícito de ferramenta (`PLANTA` / `MARCA X`), botão de ajuda (`?`) que abre o tutorial das 3 regras em overlay verde, e END Screen com botões textuais.
- **Variação B:** Menu com destaque do nível atual no topo; HUD com **3 cards permanentes de regras** na parte inferior e interação direta (**1 clique = planta**, **2 cliques = X**); overlay escuro de instruções de interação na primeira partida; seletores horizontais `OFF / ON` em Configurações; END Screen com botões amplos de ícone (Reiniciar e Avançar).

## 10. Escopo do Projeto
- **Incluído:** Arquitetura compartilhada (`Core` + `UI/Shared`), Variação A (`UI/VariantA`), Variação B (`UI/VariantB`), seletor A/B persistente sem perda de estado, 5 fases `4 × 4` pré-validadas com solução única, Desafio Diário baseado na data, persistência local via `PlayerPrefs`, UI Toolkit com Runtime Data Binding nativo da Unity 6.
- **Fora de escopo:** Geração procedural complexa, recursos online, telemetria automatizada, sistemas de áudio/vibração nativos completos e menus extras de acessibilidade.
