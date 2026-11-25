# Documentação de Interface — Variação A (`UI_VARIANTE_A.md`)

## 1. Referências Visuais e Versão da Unity
- **Engine:** Unity `6000.0.35f1` (suporte nativo a Runtime Data Binding via `Unity.Properties`, `DataBinding` e `INotifyBindablePropertyChanged`).
- **Assets:** Extraídos de `Assets/SudokuVerde_Assets/PNG/` (43 PNGs originais das pranchetas `400 × 800`, proporção `1:2`).
- **Tipografia:** A fonte `Winkle-Regular` mencionada em `Assets/SudokuVerde_Assets/LEIA-ME.md` não está presente no projeto. Foi adotada `Assets/Feel/MMTools/Demos/MMTween/Fonts/Lato/TTF/Lato-Bold.ttf` como alternativa limpa e altamente legível para o público 60+.

## 2. Descrição das Telas (Variação A)
1. **Menu Inicial (`MainMenuView.uxml`):**
   - Fundo em degradê (`background_gradient.png`), botão de configurações no topo esquerdo, ilustração da jardineira (`illustration_garden.png`), título `"Sudoku Verde"`, contador `"SEQUENCIA DIARIA"` (`counter_base.png`) e botões largos `"JOGAR"` e `"DESAFIO DIARIO"`.
2. **HUD e Gameplay (`GameplayView.uxml` + `BoardCell.uxml`):**
   - Fundo com gramas decorativas (`background_gameplay.png`), cabeçalho com pausa/voltar, indicador do nível atual e ajuda (`?`).
   - Tabuleiro centralizado (`board_base.png`) com 16 células reutilizáveis (`cell_orange`, `cell_blue`, `cell_green`, `cell_light_green`), ícones independentes de planta (`icon_plant.png`) e X (`icon_cross.png`).
   - Barra inferior com botão alternador de ferramenta (`PLANTA` / `MARCA X`) + badge de plantas restantes e botão de dica (`DICA`) + badge de dicas restantes.
3. **Menu de Pausa (`PauseOverlayView.uxml`):**
   - Overlay verde translúcido (`overlay_green.png`) preservando o estado da partida, cartão central e botões `"CONTINUAR"`, `"REINICIAR FASE"`, `"CONFIGURACOES"` e `"VOLTAR AO MENU"`.
4. **Configurações (`SettingsView.uxml`):**
   - Fundo em degradê, botão voltar (retorna à tela de origem: Menu ou Pausa), título `"CONFIGURACOES"` e controles para `MUSICA` (`ON`/`OFF`), `VIBRA` (`ON`/`OFF`) e `VOZ` (`JOGAR`/`OFF`), persistidos em `PlayerPrefs`.
5. **Como Jogar (`TutorialOverlayView.uxml`):**
   - Overlay verde translúcido sobre o jogo com as 3 regras e diagramas originais (`tutorial_color.png`, `tutorial_row_column.png`, `tutorial_adjacency.png`).
6. **END Screen — Fase Concluída (`VictoryOverlayView.uxml`):**
   - Exibida automaticamente ao resolver o tabuleiro: overlay verde translúcido, painel `"FASE CONCLUIDA!"`, planta comemorativa com brilhos (`icon_plant_victory.png`, `sparkles_victory.png`), botão fechar, `"PROXIMA FASE"` e `"VOLTAR AO MENU"`.

## 3. Decisões de Design para o Público 60+
- **Botões amplos e espaçados:** Uso consistente de `button_wide_base` (`276 × 56 px` na referência) e células `4 × 4` grandes (~`80 × 80 px`) reduzindo erros de toque.
- **Alto contraste e legibilidade:** Textos escuros (`#231F20`) sobre fundos creme claros (`#EEF6C5`) e badges vermelhos de contagem.
- **Feedback visual direto sem poluição:** Destaque de borda verde na ferramenta ativa, borda vermelha em conflitos de regra e animação sutil de escala ao pressionar botões (`:active`) e ao concluir a fase.
- **Navegação previsível:** Retorno de Configurações volta exatamente à tela de origem (Menu ou Pausa).

## 4. Fluxo de Navegação Implementado
- `Menu Inicial` → `Gameplay` (`JOGAR`)
- `Menu Inicial` → `Desafio Diário` (`DESAFIO DIARIO`)
- `Menu Inicial` ↔ `Configurações`
- `Gameplay` ↔ `Menu de Pausa`
- `Gameplay` ↔ `Como Jogar`
- `Menu de Pausa` ↔ `Configurações`
- `Menu de Pausa` → `Reiniciar Fase` (permanece no Gameplay zerado)
- `Menu de Pausa` → `Menu Inicial`
- `Gameplay` → `END Screen` (automático ao completar)
- `END Screen` → `Próxima Fase` (`Gameplay`)
- `END Screen` → `Menu Inicial` (ou botão Fechar)

## 5. Data Bindings Implementados (Runtime Native Binding)
Todos os ViewModels herdam de `BindableViewModelBase` (`INotifyBindablePropertyChanged`) com atributos `[CreateProperty]`, sem polling em `Update()`:

| Elemento UI | ViewModel | Propriedade | Modo |
| --- | --- | --- | --- |
| `menu-streak-value` | `ProgressViewModel` | `DailyStreakText` | `ToTarget` (Unidirecional) |
| `gameplay-level-label` | `GameViewModel` | `LevelTitleText` | `ToTarget` (Unidirecional) |
| `plants-remaining-label` | `GameViewModel` | `PlantsRemainingText` | `ToTarget` (Unidirecional) |
| `hints-remaining-label` | `GameViewModel` | `HintsRemainingText` | `ToTarget` (Unidirecional) |
| `tool-mode-label` | `GameViewModel` | `ActiveToolLabel` | `ToTarget` (Unidirecional) |
| `toggle-music` | `SettingsViewModel` | `MusicEnabled` | `TwoWay` (Bidirecional) |
| `toggle-vibration` | `SettingsViewModel` | `VibrationEnabled` | `TwoWay` (Bidirecional) |
| `toggle-voice` | `SettingsViewModel` | `VoiceEnabled` | `TwoWay` (Bidirecional) |
| `music-status-label` | `SettingsViewModel` | `MusicStatusText` | `ToTarget` (Unidirecional) |
| `vibration-status-label` | `SettingsViewModel` | `VibrationStatusText` | `ToTarget` (Unidirecional) |
| `voice-status-label` | `SettingsViewModel` | `VoiceStatusText` | `ToTarget` (Unidirecional) |

## 6. Limitações Documentadas
- **Áudio e Vibração:** Não há arquivos de música, narração por voz ou plugin de háptica configurado nesta entrega acadêmica. Os estados de `MUSICA`, `VIBRA` e `VOZ` são persistidos em `PlayerPrefs` e mantidos fielmente entre telas, sem simular efeitos inexistentes.
- **Fonte original:** `Winkle-Regular` substituída por `Lato-Bold`.
