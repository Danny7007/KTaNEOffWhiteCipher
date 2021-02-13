﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System;
using System.Linq;
using Words;
public class greenCipher : MonoBehaviour
{

    public TextMesh[] screenTexts;
    public KMBombInfo Bomb;
    public KMBombModule module;
    public AudioClip[] sounds;
    public KMAudio Audio;
    public TextMesh submitText;
    private List<List<string>> wordList = new List<List<string>>();
    private string[][] pages;
    private string answer;
    private int page;
    private bool submitScreen;
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    public KMSelectable leftArrow;
    public KMSelectable rightArrow;
    public KMSelectable submit;
    public KMSelectable[] keyboard;
    void Awake()
    {
        moduleId = moduleIdCounter++;
        Data data = new Data();
        wordList = data.allWords;
        leftArrow.OnInteract += delegate () { left(leftArrow); return false; };
        rightArrow.OnInteract += delegate () { right(rightArrow); return false; };
        submit.OnInteract += delegate () { submitWord(submit); return false; };
        foreach (KMSelectable keybutton in keyboard)
        {
            KMSelectable pressedButton = keybutton;
            pressedButton.OnInteract += delegate () { letterPress(pressedButton); return false; };
        }
    }
    // Use this for initialization
    void Start()
    {
        submitText.text = "1";
        //Generating random word
        answer = wordList[2][UnityEngine.Random.Range(0, wordList[2].Count)].ToUpper();
        wordList[2].Remove(answer);
        Debug.LogFormat("[Green Cipher #{0}] Generated Word: {1}", moduleId, answer);
        pages = new string[2][];
        pages[0] = new string[3];
        pages[1] = new string[3];
        pages[0][0] = "";
        pages[0][1] = "";
        pages[0][2] = "";
        string encrypt = greencipher(answer);
        pages[0][0] = encrypt.ToUpper();
        page = 0;
        getScreens();
    }
    string greencipher(string word)
    {
        Debug.LogFormat("[Green Cipher #{0}] Begin Mechanical Encryption", moduleId);
        string kw = wordList[2][UnityEngine.Random.Range(0, wordList[2].Count)].ToUpper();
        wordList[2].Remove(kw);
        string encrypt = MechanicalEnc(word.ToUpper(), kw.ToUpper());
        Debug.LogFormat("[Green Cipher #{0}] Begin Ragbaby Encryption", moduleId);
        encrypt = RagbabyEnc(encrypt.ToUpper());
        Debug.LogFormat("[Green Cipher #{0}] Begin Homophonic Encryption", moduleId);
        kw = HomophonicEnc(kw);
        pages[0][1] = kw.Substring(0, 6);
        pages[0][2] = kw.Substring(6);
        return encrypt;
    }
    string HomophonicEnc(string word)
    {
        string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string kw = alpha[UnityEngine.Random.Range(0, 26)] + "" + alpha[UnityEngine.Random.Range(0, 26)] + "" + alpha[UnityEngine.Random.Range(0, 26)];
        int[][] nums = new int[3][];
        for (int aa = 0; aa < nums.Length; aa++)
        {
            nums[aa] = new int[26];
            for (int bb = 0; bb < 26; bb++)
                nums[aa][bb] = (bb + 1) + (26 * aa);
        }

        for (int cc = 0; cc < 3; cc++)
        {
            int rotations = alpha.IndexOf(kw[cc]);
            for (int dd = 0; dd < rotations; dd++)
                nums[cc] = HomophonicRot(nums[cc]);
            Debug.LogFormat("[Green Cipher #{0}] Row {1}: {2}", moduleId, cc + 1, kw[cc]);
        }
        string rows = "001122";
        string tens = "";
        string ones = "";
        for (int dd = 0; dd < 6; dd++)
        {
            int cur = UnityEngine.Random.Range(0, rows.Length);
            char row = rows[cur];
            string num = nums[row - '0'][alpha.IndexOf(word[dd])] + "";
            if (num.Length == 1)
                num = "0" + num[0];
            Debug.LogFormat("[Green Cipher #{0}] {1} -> {2}", moduleId, word[dd], num);
            tens = tens + "" + num[0];
            ones = ones + "" + num[1];
            rows = rows.Substring(0, cur) + "" + rows.Substring(cur + 1);
        }
        pages[1][0] = kw.ToUpper();
        return tens + "" + ones;
    }
    int[] HomophonicRot(int[] n)
    {
        int[] c = new int[26];
        for (int aa = 1; aa < 26; aa++)
            c[aa] = n[aa - 1];
        c[0] = n[25];
        return c;
    }
    /*
    string ColumnTrans(string word)
    {
        int numcols = UnityEngine.Random.Range(0, 5) + 2;
        string temp = "123456".Substring(0, numcols);
        string nums = "";
        for(int aa = 0; aa < numcols; aa++)
        {
            int randnum = UnityEngine.Random.Range(0, temp.Length);
            nums = nums + "" + temp[randnum];
            temp = temp.Substring(0, randnum) + "" + temp.Substring(randnum + 1);
        }
        string[] columns = new string[numcols];
        for(int bb = 0; bb < 6; bb++)
        {
            columns[bb % numcols] = columns[bb % numcols] + "" + word[bb];
        }
        string encrypt = "";
        for(int cc = 0; cc < numcols; cc++)
        {
            string find = (cc + 1) + "";
            encrypt = encrypt + "" + (columns[nums.IndexOf(find[0])]);
            Debug.LogFormat("[Green Cipher #{0}] Column {1}: {2}", moduleId, cc + 1, columns[nums.IndexOf(find[0])]);
        }
        Debug.LogFormat("[Green Cipher #{0}] {1} -> {2}", moduleId, word, encrypt);
        pages[1][1] = nums.ToUpper();
        return encrypt;
    }*/
    string RagbabyEnc(string word)
    {
        int length = UnityEngine.Random.Range(0, wordList.Count);
        string kw = wordList[length][UnityEngine.Random.Range(0, wordList[length].Count)].ToUpper();
        wordList[length].Remove(kw);
        pages[1][1] = kw.ToUpper();
        string key = getKey(kw.ToUpper(), "ABCDEFGHIJKLMNOPQRSTUVWXYZ", Bomb.GetOffIndicators().Count() % 2 == 0);
        Debug.LogFormat("[Green Cipher #{0}] Generated Key: {1}", moduleId, key);
        string encrypt = "";
        for (int aa = 0; aa < 6; aa++)
        {
            encrypt = encrypt + "" + key[(key.IndexOf(word[aa]) + (aa + 1)) % 26];
            Debug.LogFormat("[Green Cipher #{0}] {1} -> {2}", moduleId, word[aa], encrypt[aa]);
        }
        return encrypt;
    }
    string MechanicalEnc(string word, string kw)
    {
        string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string[] table =
        {
            "UFHKQIPLXNZESGBVMCWJRDOTYA",
            "IWCZYMLKJODGFSQRNBTXHUEVAP",
            "WBSMEJTUCPFAHZOQLIKNYVGXRD",
            "GRINQVWOTYAJXBMHCFKLDUSZEP",
            "DLTVSUIKWCXRFJZANYHMQOGEPB",
            "FSVCEIUJKPGNTYHBLRQOXMADWZ",
            "JOCYWFPADKHIUVTSMENGQLZBRX",
            "BPHORAKNUETDZYQIMSFJGVWCXL",
            "ANDSQWTGXKFPCOVBLMYEZHRJIU",
            "AQJPBUSGWNXZVDYLETCOFHRIMK",
            "BHFTDGERXJAMUNZVYKOSPILCWQ",
            "JHUKDMSNEBICZYWLXQFPORTAVG",
            "ASNTZDBGWYILEORCQFXJPKHMVU",
            "RPCQABVLGWFENIKYMDUTSJXOZH",
            "YIXNVWQSUHFOMZDGKJPCTBELAR",
            "IMPCZLEGJARNTWSYFQDOUBKHVX",
            "JGKOXMUBAVRTFYCNPWQZESILHD",
            "SVHDBZNMKWJIEUYFXRQPLGCATO",
            "TZXGOPNBWAIYRHQLVKJSCDUEFM",
            "DJQZYWTPKIXCVABFNUEOLHSGRM",
            "CJOEDYHBNIXZRTPWGALFKUSMVQ",
            "FEHLYOBGRXQKVZUIMJTNACDPSW",
            "MOGAPTHIZXRFKLYSVDBWUQNECJ",
            "RXMSBPWOEJADIYNQLGKCTUHZFV",
            "ZJVWFBEOTKRDHSCPIGQNAYLUXM",
            "VWFXUEKRLBQTMCHSGJOZYDAPIN"
        };
        string encrypt = "";
        for (int aa = 0; aa < 6; aa++)
        {
            int row = alpha.IndexOf(kw[aa]);
            int col = alpha.IndexOf(word[aa]);
            encrypt = encrypt + "" + table[row][col];
            Debug.LogFormat("[Green Cipher #{0}] {1} + {2} -> {3}", moduleId, kw[aa], word[aa], encrypt[aa]);
        }
        pages[1][0] = kw.ToUpper();
        return encrypt;
    }
    string getKey(string k, string alpha, bool start)
    {
        for (int aa = 0; aa < k.Length; aa++)
        {
            for (int bb = aa + 1; bb < k.Length; bb++)
            {
                if (k[aa] == k[bb])
                {
                    k = k.Substring(0, bb) + "" + k.Substring(bb + 1);
                    bb--;
                }
            }
            alpha = alpha.Replace(k[aa].ToString(), "");
        }
        if (start)
            return (k + "" + alpha);
        else
            return (alpha + "" + k);
    }
    int correction(int p, int max)
    {
        while (p < 0)
            p += max;
        while (p >= max)
            p -= max;
        return p;
    }
    void left(KMSelectable arrow)
    {
        if (!moduleSolved)
        {
            Audio.PlaySoundAtTransform(sounds[0].name, transform);
            submitScreen = false;
            arrow.AddInteractionPunch();
            page--;
            page = correction(page, pages.Length);
            getScreens();
        }
    }
    void right(KMSelectable arrow)
    {
        if (!moduleSolved)
        {
            Audio.PlaySoundAtTransform(sounds[0].name, transform);
            submitScreen = false;
            arrow.AddInteractionPunch();
            page++;
            page = correction(page, pages.Length);
            getScreens();
        }
    }
    private void getScreens()
    {
        submitText.text = (page + 1) + "";
        screenTexts[0].text = pages[page][0];
        screenTexts[1].text = pages[page][1];
        screenTexts[2].text = pages[page][2];
        if (page == 0)
        {
            screenTexts[0].fontSize = 40;
            screenTexts[1].fontSize = 40;
            screenTexts[2].fontSize = 40;
        }
        else
        {
            screenTexts[0].fontSize = 40;
            screenTexts[1].fontSize = 35;
            screenTexts[2].fontSize = 40;
        }
    }
    void submitWord(KMSelectable submitButton)
    {
        if (!moduleSolved)
        {
            submitButton.AddInteractionPunch();
            if (screenTexts[2].text.Equals(answer))
            {
                Audio.PlaySoundAtTransform(sounds[2].name, transform);
                module.HandlePass();
                moduleSolved = true;
                screenTexts[2].text = "";
            }
            else
            {
                Audio.PlaySoundAtTransform(sounds[3].name, transform);
                module.HandleStrike();
                page = 0;
                getScreens();
                submitScreen = false;
            }
        }
    }
    void letterPress(KMSelectable pressed)
    {
        if (!moduleSolved)
        {
            pressed.AddInteractionPunch();
            Audio.PlaySoundAtTransform(sounds[1].name, transform);
            if (submitScreen)
            {
                if (screenTexts[2].text.Length < 6)
                {
                    screenTexts[2].text = screenTexts[2].text + "" + pressed.GetComponentInChildren<TextMesh>().text;
                }
            }
            else
            {
                submitText.text = "SUB";
                screenTexts[0].text = "";
                screenTexts[1].text = "";
                screenTexts[2].text = pressed.GetComponentInChildren<TextMesh>().text;
                screenTexts[2].fontSize = 40;
                submitScreen = true;
            }
        }
    }
#pragma warning disable 414
    private string TwitchHelpMessage = "Move to other screens using !{0} right|left|r|l|. Submit the decrypted word with !{0} submit qwertyuiopasdfghjklzxcvbnm";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {

        if (command.EqualsIgnoreCase("right") || command.EqualsIgnoreCase("r"))
        {
            yield return null;
            rightArrow.OnInteract();
            yield return new WaitForSeconds(0.1f);

        }
        if (command.EqualsIgnoreCase("left") || command.EqualsIgnoreCase("l"))
        {
            yield return null;
            leftArrow.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2 || !split[0].Equals("SUBMIT") || split[1].Length != 6) yield break;
        int[] buttons = split[1].Select(getPositionFromChar).ToArray();
        if (buttons.Any(x => x < 0)) yield break;

        yield return null;

        yield return new WaitForSeconds(0.1f);
        foreach (char let in split[1])
        {
            keyboard[getPositionFromChar(let)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (submitScreen && !answer.StartsWith(screenTexts[2].text))
        {
            KMSelectable[] arrows = new KMSelectable[] { leftArrow, rightArrow };
            arrows[UnityEngine.Random.Range(0, 2)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        int start = submitScreen ? screenTexts[2].text.Length : 0;
        for (int i = start; i < 6; i++)
        {
            keyboard[getPositionFromChar(answer[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    private int getPositionFromChar(char c)
    {
        return "QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c);
    }
}
