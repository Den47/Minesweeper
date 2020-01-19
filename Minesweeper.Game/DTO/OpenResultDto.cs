using System.Collections.Generic;

namespace Minesweeper.Game.DTO
{
	public class OpenResultDto
	{
		public OpenResultDto(GameState state, IEnumerable<AddressDto> opennedAddresses)
		{
			State = state;
			OpennedAddresses = new List<AddressDto>(opennedAddresses);
		}

		public GameState State { get; }

		public IEnumerable<AddressDto> OpennedAddresses { get; }
	}
}
