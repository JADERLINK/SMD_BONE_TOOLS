# SMD_BONE_TOOLS
Utility tools to fix SMD file bones (StudioModel Data)

**Translate from Portuguese Brazil**

*Update 1.1*
<br>Adicionada nova tool, atualização das libs, e melhorias no código.

### SMD_SKELETON_CHANGER.exe

Faz o mesmo que a tool "GC_GC_Skeleton_Changer.exe" do Son of Persia, porém essa tool mantém as posições modificadas dos Bones, diferente da versão do Persia, que colocava as posições originais.
<br>Basicamente, o que ele faz é corrigir os "ossos" dos arquivos SMD exportados de programas de modelagem 3D.
<br>Uso:
<br>**SMD_SKELETON_CHANGER.exe "original-unedited.smd" "edited.smd"**
<br> Output file: edited.changed.smd

### SMD_FIX_BONE_ID.exe

Faz o mesmo que o de cima, porém só precisa do arquivo modificado, mas precisa que o nome dos bones siga esses padrões: "BONE_\<N\>" ou "bone_\<N\>" ou "\*b0ne_\<N\>", exemplos:
<br>BONE_000, BONE_001, BONE_002, BONE_003 ...
<br>bone_0, bone_1, bone_2, bone_3 ...
<br>file_b0ne_000, file_B0NE_1, FILEB0NE_002 ...
<br>Uso:
<br>**SMD_FIX_BONE_ID.exe "edited.smd"**
<br>Nota: o ID atribuído é o que está no nome do bone, ex: BONE_003 recebe o ID 3
<br>Output file: Na versão 1.0 cria o arquivo "edited.fixed.smd" e na versão 1.1 sobrescreve o arquivo original.

### SMD_REPLACE_BONE.exe
Essa tool serve para renomear e mudar os IDs dos bones.
<br> A utilidade dessa tool é converter os SMDs do RE4 2007 para o RE4 UHD e vice-versa.
<br> Mas para isso você vai precisar de um arquivo "replacebone", esse arquivo informa qual bone vira, qual bone. Deixei dois exemplos junto com o programa.
<br>Uso:
<br>**SMD_REPLACE_BONE.exe "edited.smd" "your.replacebone"**
<br> Output file: edited.replaced.smd

### your.replacebone

Cada linha é usada para representar uma substituição de bone, porém linhas iniciadas com # são comentários.
<br> A ordem correta é:
<br>nome_do_bone_no_SMD|novo_ID_do_bone|novo_nome_do_bone|parent_ID
<br>Exemplos:
<br># Do 2007 para UHD
<br>born__00|0|BONE_000|-1
<br>born__01|1|BONE_001|0
<br>born__02|2|BONE_002|1
<br># Do UHD para 2007
<br>BONE_000|4|born__00|3
<br>BONE_001|5|born__01|4
<br>BONE_002|6|born__02|5
<br>BONE_003|7|born__03|6

### SMD_NEW_BONE_NAME.exe
A utilidade dessa tool é renomear o nome do bone, para colocar junto ao nome original do bone o nome do arquivo, para quando o arquivo smd for importado pelo blender não sobrepor os bones, porém no blender todos os bones ficam em um lugar só, e ao exportar para smd, o blender exporta todos os bones até os dos outros arquivos, o que faz com que tenha mais bones no arquivo do que deveria, então na prática essa tool não é útil.
<br>Uso:
<br>**SMD_NEW_BONE_NAME.exe "edited.smd"**
<br> Output file: ele sobrescreve o arquivo.

**At.te: JADERLINK**
<br>2025-10-18