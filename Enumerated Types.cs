using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex_2048
{
	public enum enuHex2048_FSM 
	{ 
		idle, 
		Game_New, 
		Game_Over, 
		Form_Moving, 
		Form_Resizing, 
		Tile_Moving, 
		Tiles_Adding_Init, 
		Tiles_Adding_Animate, 
		Tiles_GatherLike, 
		Tiles_GatherLike_Flash, 
		Tiles_RemoveLike, 
		Tiles_RemoveFinal, 
		_num 
	};


}
