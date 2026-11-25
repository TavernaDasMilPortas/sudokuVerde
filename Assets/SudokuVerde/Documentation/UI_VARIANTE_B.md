# Documentação de Interface — Variação B (`UI_VARIANTE_B.md`)

## 1. Referências Visuais e Assets
- **Referência principal:** `sodoku sketch ui 2.ai` (5 pranchetas preenchidas exportadas em `Assets/SudokuVerde_Assets/SudokuVerde_VariantB_Complemento/Referencias/`). A 6ª prancheta vazia do arquivo não representa uma tela.
- **Assets reaproveitados da Variação A:** `background_gradient.png`, `background_gameplay.png`, `illustration_garden.png`, `icon_plant.png`, `icon_hint.png`, `icon_cross.png`, `icon_back.png`, `icon_plant_victory.png`, `sparkles_victory.png`, `badge_base.png`, `button_wide_base.png`, `header_settings_base.png` e células coloridas (`cell_orange`, `cell_blue`, `cell_green`, `cell_light_green`).
- **Assets específicos da Variação B (`SudokuVerde_VariantB_Complemento/PNG/`):** `variant_b_header_level_base.png`, `variant_b_board_base.png`, `variant_b_button_square_base.png`, `variant_b_icon_close.png`, `variant_b_rule_card_base.png`, `variant_b_rule_color.png`, `variant_b_rule_row_column.png`, `variant_b_rule_adjacency.png`, `variant_b_overlay_dark.png`, `variant_b_instruction_card_plant_base.png`, `variant_b_instruction_card_cross_base.png`, `variant_b_instruction_icon_tile_base.png`, `variant_b_icon_cross_large.png`, `variant_b_icon_music.png`, `variant_b_icon_vibration.png`, `variant_b_toggle_off_base.png`, `variant_b_toggle_on_base.png`, `variant_b_victory_panel_base.png`, `variant_b_victory_action_base.png`, `variant_b_icon_restart.png`, `variant_b_icon_next.png`.

## 2. Descrição das Telas (Variação B)
1. **Menu Inicial (`MainMenuViewB.uxml`):** Fundo em degradê, banner de nível atual no topo (`Nivel 1`), ilustração da jardineira, título `Sudoku Verde` e botões `JOGAR` e `configurações`. Sem sequência diária nem Desafio Diário, preservando os dados internamente.
2. **HUD e Gameplay (`GameplayViewB.uxml` + `BoardCellB.uxml`):**
   - Topo com botão quadrado de fechar/pausar à esquerda e botão quadrado de dica com contador vermelho à direita.
   - Aba central com o nível atual diretamente acima do tabuleiro (`variant_b_board_base.png`).
   - **3 cards permanentes de regras** na parte inferior (`1 planta por cor`, `1 planta por coluna e linha`, `plantas nao podem se tocar`) sempre visíveis durante a partida.
3. **Overlay de Instruções (`InstructionsOverlayViewB.uxml`):** Overlay escuro com 2 cards horizontais explicando `clique uma vez para posicionar plantas` e `clique duas vezes para posicionar x`, exibido automaticamente na primeira partida na Variação B e acessível novamente via Menu de Pausa (`COMO JOGAR`).
4. **Menu de Pausa (`PauseOverlayViewB.uxml`):** Overlay escuro com painel claro de borda verde e botões `CONTINUAR`, `REINICIAR FASE`, `COMO JOGAR`, `CONFIGURACOES` e `VOLTAR AO MENU`.
5. **Configurações (`SettingsViewB.uxml`):** Ícones de música e vibração com seletores horizontais `OFF / ON` e seletor horizontal `VERSAO A / VERSAO B` abaixo. Não exibe controle de voz (preservado internamente para a Variação A).
6. **END Screen (`VictoryOverlayViewB.uxml`):** Overlay escuro, painel central `CONCLUIDO!` com planta ampliada e brilhos, botão fechar no topo esquerdo e dois botões quadrados amplos abaixo: **Reiniciar** (sem contabilizar progresso duplicado) e **Avançar**.

## 3. Método de Interação (Clique Simples e Clique Duplo)
- **1 clique/toque:** Posiciona ou remove uma **planta**.
- **2 cliques/toques consecutivos na mesma célula** (janela configurável em código `doubleClickWindowMs = 400 ms`, entre 350 e 450 ms): Posiciona ou remove um **X**.
- Um único agendador UI Toolkit resolve o gesto sem callbacks duplicados; toques pendentes são cancelados ao trocar de tela, abrir overlay ou alternar variante, evitando vitória prematura.

## 4. Seletor A/B e Data Bindings Compartilhados
- Presente em Configurações nas **duas variantes** (`toggle-variant` ligado com `BindingMode.TwoWay` a `SettingsViewModel.IsVariantB` / `SelectedVariant`).
- A troca de variante persiste em `PlayerPrefs`, limpa todos os callbacks da árvore visual anterior (`TeardownVisualTreeBindings()`), reconstrói o `VisualTreeAsset` da nova variante, mantém Configurações aberto e preserva integralmente o tabuleiro pausado, o progresso e a tela de origem.
- Todos os ViewModels (`GameViewModel`, `SettingsViewModel`, `ProgressViewModel`, `NavigationViewModel`) são compartilhados entre as variantes A e B via Runtime Data Binding nativo da Unity 6.
