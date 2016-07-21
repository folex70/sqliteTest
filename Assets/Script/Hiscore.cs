using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Hiscore: IComparable<Hiscore>{

	public int IdScore{get;set;} 
	public string Name{get;set;} 
	public int Score{get;set;} 
	public DateTime Date{get;set;} 


	public Hiscore(int idscore, string name, int score,DateTime date)
	{
		this.IdScore = idscore;
		this.Name = name;
		this.Score = score;
		this.Date = date;

	}

	public int CompareTo(Hiscore other){

		if (other.Score < this.Score) {
			return -1;
		}
		if (other.Score > this.Score) {
			return 1;
		}
		else if (other.Date< this.Date){
			return -1;
		}
		else if (other.Date> this.Date){
			return 1;
		}
		return 0;

	}

}