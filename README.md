# SMD_BONE_TOOLS
Utility tools to fix SMD file bones (StudioModel Data)

**Translate from Portuguese Brazil**

### SMD_SKELETON_CHANGER.exe

Faz o mesmo que a tool "GC_GC_Skeleton_Changer.exe" do Son of Persia, porém essa tool mantém as posições modificadas dos Bones, diferente da versão do Persia, que colocava as posições originais.
<br>Basicamente, o que ele faz: é corrigir os "ossos" dos arquivos SMD exportados de programas de modelagem 3D.
<br>Uso:
<br>**SMD_SKELETON_CHANGER.exe "original-unedited.smd" "edited.smd"**

### SMD_FIX_BONE_ID.exe

Faz o mesmo que o de cima, porém só precisa do arquivo modificado, mas precisa que o nome dos bones siga esses padrões: "BONE_\<N\>" ou "bone_\<N\>", exemplos:
<br>BONE_000, BONE_001, BONE_002, BONE_003 ...
<br>bone_0, bone_1, bone_2, bone_3 ...
<br>Uso:
<br>**SMD_FIX_BONE_ID.exe "edited.smd"**
<br>Nota: o ID atribuído é o que está no nome do bone, ex: BONE_003 recebe o ID 3

### SMD_REPLACE_BONE.exe
Essa tool serve para renomear e mudar os IDs dos bones.
<br> A utilidade dessa tool é converter os SMDs do RE4 2007 para o RE4 UHD e vice-versa.
<br> Mas para isso você vai precisar de um arquivo "replacebone", esse arquivo informa qual bone vira, qual bone. Deixei dois exemplos junto com o programa.
<br>Uso:
<br>**SMD_REPLACE_BONE.exe "edited.smd" "your.replacebone"**

### your.replacebone

Cada linha é usada para representar uma substituição de bone, porem linhas iniciadas com # são comentários.
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

**At.te: JADERLINK**
<br>2025-02-05
