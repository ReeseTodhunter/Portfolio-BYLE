using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomTextScript : MonoBehaviour
{
    List<string> potentialTextLines = new List<string>{ "Got BYLE?",
        "In a BYLE, crocodile",
        "Try drink the BYLE, trust me bro",
        "Cats can have a little BYLE",
        "BYLE makes a good substitute for sunscreen" ,
        "BYLE can not melt steel beams",
        "BYLE moBYLE coming to a phone near you",
        "Did BYLE start covid?",
        "I do not wish to be political but has anyone seen BYLE lately?",
        "Time for BYLE",
        "BYLE racing - automoBYLE",
        "RIP BYLE 2",
        "BYLE forever after",
        "When the BYLE",
        "BYLE time",
        "What happens when I use BYLE",
        "Don't BYLE and drive",
        "Try to hold your breathe why BYLEing",
        "My minds telling me no... but my BYLE",
        "We'll BYLE again, don't know where, don't know when",
        "I'm beginning to feel like a BYLE god",
        "Never gonna BYLE you up",
        "I know I BYLE in line until the...",
        "I know you BYLE me, I know you care",
        "It's been a long time, without you my BYLE",
        "BYLE lips, BYLE face, BYLEing in the snowflakes",
        "We were just BYLE when we fell in love",
        "That hurt me, right in the BYLE",
        "BYLE for smash bros",
        "BYLE for smash bros pretty please?",
        "BYLE pumps through these veins",
        "She said do you love me, I tell her only BYLE",
        "Let sleeping dogs BYLE",
        "Ewan - BYLEs worst nightmare",
        "Jak can't get to level 3 of BYLE",
        "BYLEy jean is not my lover",
        "BYLE is good, BYLE is great",
        "On the 7th day God said - let there be BYLE",
        "BYLE makes a good chaser (true story)",
        "What happens on level 36?",
        "The BYLE smells different on a certain level",
        "Imma show you how BYLE I am",
        "I could be your BYLE if you let me",
        "Stop, hey hey, whats that sound? It is BYLE",
        "BYLEing of issac",
        "Well I don't want the BYLE to see me",
        "BYLE does super effective damage to BYLE types",
        "Womp womp, the BYLE got you",
        "You've been BYLEd, send to your friend to BYLE them",
        "BYLE cinematic universe coming soon...",
        "BYLE show coming to a TV near you",
        "Since she left, everything smells of BYLE",
        "Vote for the Crab, do it for BYLE",
        "BYLElands in the stream, that is what we are",
        "I would walk 500 BYLEs and I would walk 500 more",
        "BYLE, though your heart is aching",
        "I do not sell goods, you're just in deBYLE",
        "All that glitters is BYLE",
        "BYLE loves John Mcginn",
        "That wasn't BYLE, lets check VAR",
        "BYLE does not like 2.6 hog",
        "How many grams of protein is in BYLE?",
        "What does BYLE mean for the Osprey population?",
        "I'm BYLE and this is my pawn shop",
        "What happens in BYLE stays in BYLE",
        "I cant see me BYLEing nobody but you",
        "Soyl? more like BYLE",
        "BYLE time, come on grab your friends, we will BYLE",
        "Hey cant you see me BYLEing in here",
        "You have a choice, you can take the blue pill, or the BYLE pill",
        "Water, Earth, Fire, BYLE, everything changed when the BYLE nation attacked",
        "One BYLE to rule them all",
        "You were raising him like a BYLE to slaughter",
        "Don't lecture me Obi-wan, I see through the lies of the BYLE",
        "Only BYLE deals in absolutes",
        "Avoid bright light, Dont get them wet, and dont let them BYLE past midnight",
        "We are drinking the BYLE potion at 3am in the morning!",
        "Lets sit around the BYLEfire, and sing our BYLEfire song.",
        "Did you know they made a third gremlins movie? BYLE",
        "I know, your tired, of BYLEing, of BYLEing with no body to BYLE",
        "Playing BYLE makes you 150 percent cooler than the average D&D player",
        "Why can't I BYLE you in the street, why can't I BYLE you on the dancefloor",
        "BYLE, hoo, ha, what is it good for, absolutely nothing",
        "The animation discrepancy (BYLE) between stuart little 2 and 3 is dramatic",
        "The parents in stuart little were awful, how would you feel if you were the BYLE",
        "What if you fed BYLE to catdog from catdog",
        "Pinky and the brain, has absoluetly nothing to do with BYLE",
        "Early game minecraft < Late game BYLE colosseum",
        "Wait, Geek is big Z?!? What the BYLE",
        "Breaking Bad? eh, not as good as the wire or BYLE",
        "30 percent of viewers arn't actually subscribed to BYLE on youtube",
        "BYLE dev may be over, but atleast he dropped vultures",
        "Don't like BYLE? try SOYL!",
        "My favourite bugs? Woodlice or maybe BYLE",
        "Why use one grid when you can use two, BYLE moment.",
        "BYLE says - Don't touch the iron when its been used reccently",
        "Automatic reload nearly killed BYLE hahaha",
        "In west BYLEadelphia born and raised, in the playground is where...",
        "Her name was BYLE, she was a showgirl, but that was 30 years ago...",
        "Oooh what a feeling, when we're BYLEing on the ceiling",
        "Who should we cast to play the shop keeper in live action BYLE?",
        "Reality is a illusion, the universe is a hologram, buy gold, BYLE",
        "I don't know if I set the for loop properly... BYLE",
        "Who's gonna carry the BYLE!",
        "BYLEnuts roasting on a open BYLE, BYLE frost nipping at your BYLE",
        "One of the BYLE devs got 9 likes on tinder!!!"


    };

    public TMPro.TextMeshProUGUI DisplayText;


    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, potentialTextLines.Count);

        DisplayText.text = potentialTextLines[rand];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
