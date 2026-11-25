# Sudoku Verde — assets para Unity

43 PNGs e 37 SVGs. Pranchetas originais: 400 × 800, proporção 1:2. PNGs vetoriais rasterizados em 4x (fundo do jogo em 3x), com transparência onde aplicável. SVGs preservam os contornos extraídos da representação PDF do Illustrator.

## Montagem UI Toolkit
- Menu: background_gradient, illustration_garden, button_icon_base + icon_settings, counter_base, button_wide_base. Título e textos via Label/Button.
- Configurações: background_gradient, button_icon_base + icon_back, header_settings_base, button_wide_base. Estados ON/OFF e texto de voz via código.
- Jogo: background_gameplay (já inclui gramas), header_base, button_icon_base + icon_back/icon_help, board_base, células independentes (cell_green/orange/blue), icon_cross e icon_plant como filhos das células. A referência mostra 4 colunas × 5 linhas, não uma regra de geração de níveis. Ações: button_action_base + icon_plant/icon_hint, badge_base + Label numérico.
- Tutorial: sobrepor overlay_green sobre o jogo; header, painéis tutorial e os três diagramas tutorial_*. Os textos devem ser Labels.
- Vitória: overlay_green, button_icon_base + icon_close, panel_victory_title, panel_victory_reward, icon_plant_victory e sparkles_victory (ou brilhos separados). O texto de vitória permanece editável.

## Importação
Copiar PNG/ para Assets/UI/SudokuVerde/Textures. Texture Type: Sprite (2D and UI) ou Default para background-image; Alpha Is Transparency, sRGB, sem mipmaps, Filter Mode Bilinear; evitar compressão agressiva. Para SVG, usar suporte Vector Graphics compatível com a versão da Unity; PNG é a alternativa direta. Não importar as duas versões sem necessidade. Para bases redimensionáveis, usar 9-slice ou borda/raio USS; evitar distorção dos cantos. Ajustar escalas em PanelSettings com resolução de referência 400×800 (ou 800×1600). Os PNGs têm pequena margem transparente.

## Tipografia e diferenças
A fonte indicada na representação PDF é Winkle-Regular, mas a renderização local substitui os glifos por uma fonte genérica, diferente da captura original. Obter o arquivo licenciado da fonte original separadamente e configurar no UI Toolkit. Nenhum arquivo de fonte incluído. O X e o fechar foram recriados em vetor com pontas arredondadas a partir da referência visual, não extraídos como contornos originais. Os diagramas são composições dos assets; o degradê foi amostrado da prancheta de configurações; o overlay foi aproximado. A ilustração original rasterizada teve o branco conectado às bordas removido e foi reduzida para até 1800px. Os painéis de vitória foram exportados sem a transparência da composição sobreposta, que pode ser ajustada no USS. As referências incluem as cinco pranchetas renderizadas e a captura enviada. Não foram executados testes na Unity.

manifest.json registra coordenadas de origem e dimensões dos itens vetoriais, incluindo os dois X recriados. Para a versão com gramas separadas, use o fundo verde sólido #afd46e e os assets grass_*. 
