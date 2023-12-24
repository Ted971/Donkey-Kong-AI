using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class MAIro : IComparable<MAIro>
{
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights;
    private float[] activations;

    public float fitness;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public MAIro(int[] layers){
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++){            
            this.layers[i] = layers[i];        
        } 
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    public void InitAll(int[] layers){
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++){            
            this.layers[i] = layers[i];        
        } 
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for(int i=0; i < layers.Length; i++){
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();        
        for (int i = 0; i < layers.Length; i++){            
            float[] bias = new float[layers[i]];            
            for (int j = 0; j < layers[i]; j++){                
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);            
            }            
            biasList.Add(bias);        
        }        
        biases = biasList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();        
        for (int i = 1; i < layers.Length; i++){            
            List<float[]> layerWeightsList = new List<float[]>();   
            int neuronsInPreviousLayer = layers[i - 1];            
            for (int j = 0; j < neurons[i].Length; j++){                 
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++){                                      
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); 
                }               
                layerWeightsList.Add(neuronWeights);            
            }            
            weightsList.Add(layerWeightsList.ToArray());        
        }        
        weights = weightsList.ToArray();
    }

    public float Activate(float value)    
    {        
        return (float)Math.Tanh(value);    
    }

    public float[] FeedForward(float[] inputs)    
    {        
        for (int i = 0; i < inputs.Length; i++){     
            neurons[0][i] = inputs[i];        
        }        
        for (int i = 1; i < layers.Length; i++){            
            int layer = i - 1;            
            for (int j = 0; j < neurons[i].Length; j++){                
                float value = 0f;               
                for (int k = 0; k < neurons[i - 1].Length; k++)  
                {                    
                    value += weights[i - 1][j][k] * neurons[i - 1][k];      
                }                
                neurons[i][j] = Activate(value + biases[i][j]);            
            }        
        }        
        return neurons[neurons.Length - 1];    
    }

    public int CompareTo(MAIro other){
        if (other == null){
            return 1;
        }
        if(fitness > other.fitness){
            return 1;
        }
        else if (fitness < other.fitness){
            return -1;
        }
        else{
            return 0;
        }
    }

    //this loads the biases and weights from within a file into the neural network.
    public void Load(string path){
        TextReader tr = new StreamReader(path);
        int nLines = (int)new FileInfo(path).Length;
        string[] listLines = new string[nLines];
        int index = 1;
        for (int i = 1; i < nLines; i++){
            listLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0){
            for (int i = 0; i < biases.Length; i++){
                for (int j = 0; j < biases[i].Length; j++){
                    biases[i][j] = float.Parse(listLines[index]);
                    index++;
                }
            }
            for (int i = 0; i < weights.Length; i++){
                for (int j = 0; j < weights[i].Length; j++){
                    for (int k = 0; k < weights[i][j].Length; k++){
                        weights[i][j][k] = float.Parse(listLines[index]);
                        index++;
                    }
                }
            }
        }
    }

    public void Mutate(int chance, float val){
        for (int i = 0; i < biases.Length; i++){
            for (int j = 0; j < biases[i].Length; j++){
                biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++){
            for (int j = 0; j < weights[i].Length; j++){
                for (int k = 0; k < weights[i][j].Length; k++){
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];
                }
            }
        }
    }

    public MAIro Copy(MAIro other){
        for (int i = 0; i < biases.Length; i++){
            for (int j = 0; j < biases[i].Length; j++){
                other.biases[i][j] = biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++){
            for (int j = 0; j < weights[i].Length; j++){
                for (int k = 0; k < weights[i][j].Length; k++){
                    other.weights[i][j][k] = weights[i][j][k];
                }
            }
        }
        return other;
    }

    public void Save(string path){
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path, true);

        for (int i = 0; i < biases.Length; i++){
            for (int j = 0; j < biases[i].Length; j++){
                writer.WriteLine(biases[i][j]);
            }
        }
        for (int i = 0; i < weights.Length; i++){
            for (int j = 0; j < weights[i].Length; j++){
                for (int k = 0; k < weights[i][j].Length; k++){
                    writer.WriteLine(weights[i][j][k]);
                }
            }
        }
        writer.Close();
    }
}
