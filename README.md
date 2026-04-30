[![License](https://img.shields.io/badge/license-CC--BY--NC%204.0-lightgrey)](https://creativecommons.org/licenses/by-nc/4.0/)

# 🚀 Diogenes Project

## 🧠 Présentation

**Diogenes Project** est un prototype de jeu développé sur Unity.  
Je m’en sers principalement pour expérimenter différents systèmes comme la génération procédurale, le pathfinding et la gestion d’un monde basé sur une grille.

L’idée derrière ce projet, c’est moins de faire un “jeu fini” que de construire des bases techniques solides, réutilisables et évolutives.

---

## 🎮 Fonctionnalités principales

### 🌍 Génération procédurale
La map est générée via une pipeline composée de plusieurs étapes indépendantes.  
Chaque génération peut être reproduite grâce à un système de seed.

J’ai aussi ajouté une interface avec UI Toolkit pour modifier les paramètres (seed, génération, etc.) directement en jeu.

---

### 🧭 Pathfinding
Le pathfinding est basé sur un A*, mais adapté aux contraintes du jeu.

Par exemple :
- déplacement horizontal
- montée sur certains blocs
- utilisation d’échelles pour atteindre différentes hauteurs

Le but était d’avoir un système qui respecte les règles du gameplay, pas juste un algo théorique.

---

### ⚙️ Système de chargement
Le loading est découpé en plusieurs étapes, chacune avec un poids.  
Cela permet d’avoir une progression globale plus représentative.

Chaque étape communique avec un système de reporting pour mettre à jour l’interface en temps réel.

---

### 💾 Sauvegarde / chargement
Le monde peut être sauvegardé et chargé via un système JSON.

J’ai structuré cela avec des classes dédiées pour garder quelque chose de propre et extensible.

---

## 🧩 Organisation du projet

J’ai essayé de garder une structure claire :

- `Map` → gestion du monde  
- `Pathfinding` → navigation  
- `Loading` → initialisation  
- `Input / Controller` → contrôles  
- `UI` → interface  
- `Manager` → logique globale  

---

## 🏗️ Approche technique

Sur ce projet, j’ai surtout voulu travailler sur :
- la séparation des responsabilités  
- des systèmes modulaires (pipeline, steps, interfaces)  
- éviter les scripts monolithiques  

Par exemple :
- la génération fonctionne par étapes indépendantes  
- le loading suit le même principe  
- le pathfinding intègre directement les règles du jeu  

---

## 🛠️ Technologies

- Unity (C#)  
- UI Toolkit  
- JSON pour la sauvegarde  
- ScriptableObjects pour la configuration  

---

## ▶️ Lancer le projet

```bash
git clone https://github.com/AbominableSandwish/DiogenesProject.git
```
## 🎥 Aperçu
![Electricity](Docs/ElectricityV2.gif)

## 🔍 Technical Highlights

- Pipeline de génération modulaire (ScriptableObject + Steps)
- Système de loading pondéré avec progression globale
- Pathfinding avec contraintes gameplay (climb, ladder, obstacles)
- Architecture orientée systèmes et découplage
