# Arruma Contatos, criado por: Lucas Zimerman Fraulob

------------------------------------------------------
Você precisa do visual studio community para compilar este projeto

------------------------------------------------------

Adicionar o suporte de alterar os contatos:

1) Adicionar a linha abaixo dentro do package:
xmlns:r="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"

2) adicionar um "r" no Package, dentro do IgnorableNamespaces (deixando um espaço entre o 'r' e as outras strings)

3)Em Capabilities, adicionar as seguintes linhas: 

< r:Capability Name="contactsSystem"/>
