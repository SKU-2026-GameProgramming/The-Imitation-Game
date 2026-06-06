using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CipherManager : MonoBehaviour
{
    public TextMeshProUGUI[] cipherTexts = new TextMeshProUGUI[4];
    public int day = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Encrypt(List<Province> provinces, int[] keys, int count)
    {
        
        for(int i = 0; i < count; i++)
        {
            Debug.Log(provinces[i].nodeName);
            string cipher = Extract4LettersShuffled(provinces[i].nodeName);
            cipherTexts[i].text = cipher;

            //for (int j = 0; j < 4; j++)
            //    cipher[j] = (char)(((int)cipher[j] - 65 + keys[i]) % 26 + 65);

            //Debug.Log(cipher);
            //cipherTexts[i].text = new string(cipher);
        }
    }

    public string Extract4LettersShuffled(string text)
    {
        text = text.ToUpper();

        if (text.Length <= 4)
            return text;

        List<int> indices = new List<int>();

        while (indices.Count < 4)
        {
            int index = Random.Range(0, text.Length);

            if (!indices.Contains(index))
                indices.Add(index);
        }

        // 핵심: Sort() 하지 않음
        string result = "";

        foreach (int i in indices)
        {
            result += text[i];
        }

        return result;
    }
}
