using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private float timeFrame;
    [SerializeField]
    private int populationSize;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    [Range(0.0001f, 1f)]private float mutationChance = 0.01f;
    [SerializeField]
    [Range(0f, 1f)]private float mutationStrength = 0.5f;
    [SerializeField]
     [Range(0.1f, 10f)]private float gameSpeed = 1f;

    private int[] layers = new int[3] {9,5,3}; //9 inputs, 5 magic hidden layers, 3 outputs : jump, walk, climb

    public List<MAIro> networks;
    private List<Player> marios;
    private static bool botsCreated;

    // Start is called before the first frame update
    void Start()
    {

            InitNetworks();
            InvokeRepeating(nameof(CreateBots),0.1f, timeFrame);
        
    }

    public void InitNetworks()
    {
        networks = new List<MAIro>();
        for (int i = 0; i < populationSize; i++){
            MAIro net = new MAIro(layers);
            //net.Load("Assets/Pre-trained.txt");
            networks.Add(net);
        }
    }

    public void CreateBots(){
        Time.timeScale = gameSpeed;
        Barrel[] barrels = FindObjectsOfType<Barrel>();
        for(int j = 0;j < barrels.Length; j++){
            barrels[j].Restart();
        }
        if(marios != null){
            for (int i = 0; i < marios.Count; i++){
                if(marios[i] != null){
                    Destroy(marios[i].gameObject);
                }
            }
            SortNetworks();
        }

        marios = new List<Player>();
        for (int i = 0; i < populationSize; i++){
            int networkIndex = i % networks.Count;
            Player mario = Instantiate(prefab, this.transform.position, Quaternion.identity).GetComponent<Player>();
            if (mario != null){
                if (networks.Count > 0)
                {
                    // Make sure i is within the bounds of the networks array
                    i = Mathf.Clamp(i, 0, networks.Count - 1);
                    //Debug.Log(networkIndex);
                    //Debug.Log(networks[networkIndex]);

                    mario.network = networks[networkIndex];
                }
                else
                {
        Debug.LogError("The 'networks' list is empty. Make sure it is properly initialized.");
    }
}
            //mario.network = networks[i];
            marios.Add(mario);
        }
    }

    public void SortNetworks()
    {
        float highestF = -100;
        for (int i = 0; i < populationSize; i++){
            marios[i].UpdateFitness();
        }
        networks.Sort();
        for(int i = 0; i<populationSize;i++){
            if(networks[i].fitness > highestF){
                highestF = networks[i].fitness;
            }
        }
        Debug.Log(highestF);
        networks[populationSize - 1].Save("Assets/Save.txt");
        for (int i = 0; i < populationSize/2; i++){
            networks[i] = networks[i + populationSize / 2].Copy(new MAIro(layers));
            networks[i].Mutate((int)(1/mutationChance), mutationStrength);
        }
    }

    private void FixedUpdate(){
        Restart();
    }

    private void Restart(){
        int deaths = 0;
        if(marios != null){
            for (int i = 0; i < marios.Count; i++){
                if (!marios[i].isActiveAndEnabled){
                    deaths++;
                    if(deaths == populationSize){ 
                        CancelInvoke();
                        CreateBots();
                        InvokeRepeating(nameof(CreateBots),timeFrame, timeFrame);
                    }
                }
            }
        }

    }
}
