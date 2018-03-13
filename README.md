# Projet Reseau QuestFest

Pitch : Jeu multijoueur censé être un petit RPG textuel, seul la partie "combat" fonctionne.

Objets synchronisés : Classe personnelle nommée "Character" faites, la classe "Quête" était censé être de la partie.


##Système réseau

Système de hosting chez un client, permettant d'éviter d'avoir un serveur distant, et permettant ainsi à l'hôte de rejoindre 
la partie en tant que "Maitre du jeu". 

Network Manager à deux niveaux d'exécution, en fonction de si le client est host ou non.

Gestion des paquets par une callback dédiée et paquets composés d'un array d'octets contenant une entête de type de message 
basé sur une énumération. 

(Système clairement à refaire, ou non justement, à ne jamais refaire)

Outils d'envoi des paquets accessibles depuis le network manager, pour pouvoir envoyer des paquets depuis n'importe quel endroit du code

Gestion des connexions par l'hôte jusqu'au lancement de la partie (était également prévu un système de taille de salle, malheureusement 
abandonné par manque de skills évident)

Réponse du serveur aux messages clients avec déconnection de la part du client si le serveur ne répond pas

Et inversement, déconnection des clients par le serveur si ceux si ne communiquent plus pendant un certains temps. 


##Système de jeu

Une seule scène, en UI

Transition entre les différents éléments du "jeu" 

3 attaques possibles, basés sur les stats des joueurs

Voilà voilà....
