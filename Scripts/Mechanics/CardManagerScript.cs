using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public enum AbilityType
    {
        NO_ABILITY,
        INSTANT_ACTIVE,
        DOUBLE_ATTACK,
        SHIELD,
        PROVOCATION,
        REGENERATION_EACH_TURN,
        COUNTER_ATTACK
    }

    public enum SpellType
    {
        NO_SPELL,
        HEAL_ALLY_FIELD_CARDS,
        DAMAGE_ENEMY_FIELD_CARDS,
        HEAL_ALLY_HERO,
        DAMAGE_ENEMY_HERO,
        HEAL_ALLY_CARD,
        DAMAGE_ENEMY_CARD,
        SHIELD_ON_ALLY_CARD,
        PROVOCATION_ON_ALLY_CARD,
        BUFF_CARD_DAMAGE,
        DEBUFF_CARD_DAMAGE
    }

    public enum TargetType
    {
        NO_TARGET,
        ALLY_CARD_TARGET,
        ENEMY_CARD_TARGET
    }

    public string Name;
    public Sprite Logo;
    public int Attack, Defence, ManaCost;
    public bool CanAttack;
    public bool IsPlaced;

    public List<AbilityType> Abilities;
    public SpellType Spell;
    public TargetType SpellTarget;
    public int SpellValue;

    public bool IsAlive
    {
        get
        {
            return Defence > 0;
        }
    }

    public bool IsSpell
    {
        get
        {
            return Spell != SpellType.NO_SPELL;
        }
    }
    public bool HasAbility
    {
        get
        {
            return Abilities.Count > 0;
        }
    }

    public bool IsProvocation
    {
        get
        {
            return Abilities.Exists(x => x == AbilityType.PROVOCATION);
        }
    }

    public int TimesDealedDamage;
    public Card(string name, string logoPath, int attack, int defence, int manaCost, 
                AbilityType abilityType = 0, SpellType spellType = 0, int spellValue = 0, TargetType targetType = 0)
    {
        Name = name;
        Logo = Resources.Load<Sprite>(logoPath);
        Attack = attack;
        Defence = defence;
        CanAttack = false;
        ManaCost = manaCost;
        IsPlaced = false;

        Abilities = new List<AbilityType>();

        if (abilityType != 0)
        {
            Abilities.Add(abilityType);
        }

        Spell = spellType;
        SpellTarget = targetType;
        SpellValue = spellValue;

        TimesDealedDamage = 0;
    }

    public void GetDamage(int dmg)
    {
        if (dmg > 0)
        {
            if (Abilities.Exists(x => x == AbilityType.SHIELD))
            {
                Abilities.Remove(AbilityType.SHIELD);
            }
            else
            {
                Defence -= dmg;
            }    
        }


    }

    public Card GetCopy()
    {
        Card card = this;
        card.Abilities = new List<AbilityType>(Abilities);
        return card;
    }
}

public static class CardManager
{ 
    public static List<Card> AllCards = new List<Card>();
}

public class CardManagerScript : MonoBehaviour
{
    public void Awake()
    {
        CardManager.AllCards.Add(new Card("Virus", "Sprites/Cards/Virus", 5, 5, 6));
        CardManager.AllCards.Add(new Card("Net Worm", "Sprites/Cards/Net Worm", 4, 3, 5));
        CardManager.AllCards.Add(new Card("Backdoor", "Sprites/Cards/Backdoor", 3, 3, 4));
        CardManager.AllCards.Add(new Card("Miner", "Sprites/Cards/Miner", 2, 1, 2));
        CardManager.AllCards.Add(new Card("Bomber", "Sprites/Cards/Bomber", 8, 1, 7));
        CardManager.AllCards.Add(new Card("Encrypter", "Sprites/Cards/Encrypter", 1, 1, 1));

        CardManager.AllCards.Add(new Card("Provocation", "Sprites/Cards/Provocation", 1, 2, 3, Card.AbilityType.PROVOCATION));
        CardManager.AllCards.Add(new Card("Regeneration", "Sprites/Cards/Regeneration", 4, 2, 5, Card.AbilityType.REGENERATION_EACH_TURN));
        CardManager.AllCards.Add(new Card("Double Attack", "Sprites/Cards/Double Attack", 3, 2, 4, Card.AbilityType.DOUBLE_ATTACK));
        CardManager.AllCards.Add(new Card("Instant Active", "Sprites/Cards/Instant Active", 2, 1, 2, Card.AbilityType.INSTANT_ACTIVE));
        CardManager.AllCards.Add(new Card("Shield", "Sprites/Cards/Shield", 5, 1, 7, Card.AbilityType.SHIELD));
        CardManager.AllCards.Add(new Card("Counter Attack", "Sprites/Cards/Counter Attack", 3, 1, 1, Card.AbilityType.COUNTER_ATTACK));

        CardManager.AllCards.Add(new Card("HEAL_ALLY_FIELD_CARDS", "Sprites/Cards/HEAL_ALLY_FIELD_CARDS", 0, 0, 2, 0, 
            Card.SpellType.HEAL_ALLY_FIELD_CARDS, 2, Card.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new Card("DAMAGE_ENEMY_FIELD_CARDS", "Sprites/Cards/DAMAGE_ENEMY_FIELD_CARDS", 0, 0, 2, 0, 
            Card.SpellType.DAMAGE_ENEMY_FIELD_CARDS, 2, Card.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new Card("HEAL_ALLY_HERO", "Sprites/Cards/HEAL_ALLY_HERO", 0, 0, 2, 0, 
            Card.SpellType.HEAL_ALLY_HERO, 2, Card.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new Card("DAMAGE_ENEMY_HERO", "Sprites/Cards/HEAL_ALLY_HERO", 0, 0, 2, 0, 
            Card.SpellType.DAMAGE_ENEMY_HERO, 2, Card.TargetType.NO_TARGET));
        CardManager.AllCards.Add(new Card("HEAL_ALLY_CARD", "Sprites/Cards/HEAL_ALLY_CARD", 0, 0, 2, 0, 
            Card.SpellType.HEAL_ALLY_CARD, 2, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new Card("DAMAGE_ENEMY_CARD", "Sprites/Cards/DAMAGE_ENEMY_CARD", 0, 0, 2, 0, 
            Card.SpellType.DAMAGE_ENEMY_CARD, 2, Card.TargetType.ENEMY_CARD_TARGET));
        CardManager.AllCards.Add(new Card("SHIELD_ON_ALLY_CARD", "Sprites/Cards/SHIELD_ON_ALLY_CARD", 0, 0, 2, 0, 
            Card.SpellType.SHIELD_ON_ALLY_CARD, 0, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new Card("PROVOCATION_ON_ALLY_CARD", "Sprites/Cards/PROVOCATION_ON_ALLY_CARD", 0, 0, 2, 0, 
            Card.SpellType.PROVOCATION_ON_ALLY_CARD, 0, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new Card("BUFF_CARD_DAMAGE", "Sprites/Cards/BUFF_CARD_DAMAGE", 0, 0, 2, 0, 
            Card.SpellType.BUFF_CARD_DAMAGE, 2, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.AllCards.Add(new Card("DEBUFF_CARD_DAMAGE", "Sprites/Cards/DEBUFF_CARD_DAMAGE", 0, 0, 2, 0, 
            Card.SpellType.DEBUFF_CARD_DAMAGE, 2, Card.TargetType.ENEMY_CARD_TARGET));
    }
}