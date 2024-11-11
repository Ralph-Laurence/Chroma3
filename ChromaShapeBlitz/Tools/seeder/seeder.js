let levels = {
    "Easy"   : [],
    "Normal" : [],
    "Hard"   : []
};

let easy    = 50;
let normal  = 30;
let hard    = 25;

// Reward Types:
const Coin = 0;
const Gem  = 1;

// Easy
seedStages("Easy",   50, 20, 10, 15);
seedStages("Normal", 30, 25, 10, 15);
seedStages("Hard",   25, 30, 10, 15);

function seedStages(stageType, maxStages, stageTime, minStageTime, maxStageTime)
{
    for (let i = 0; i < maxStages; i++)
    {
        let stageNumber = (i + 1);
        let nextStage   = stageNumber + 1;
    
        if (nextStage > maxStages)
            nextStage = 0;                      // Next stage should not go higher than the required.
                                                // When we set it to "0", we assume that this is the last.
    
        let data = {
            "StageNumber"   : stageNumber,      // The stage number to appear in selection screen
            "StageTime"     : stageTime,        // The allotted play time in seconds.
            "MinStageTime"  : minStageTime,     // Finish under this seconds to get full stars
            "MaxStageTime"  : maxStageTime,     // Finish atleast within this seconds to get two stars
            "StarsAttained" : 0,                // How much stars were attained from that level
            "RewardAmount"  : 5,
            "RewardType"    : Coin,
            "RewardClaimed" : true,             // Is the original reward amount claimed?
            "NextStage"     : stageNumber + 1,
        };
    
        levels[stageType].push(data);
    }
}

let output = document.querySelector("#output");

output.value = JSON.stringify(levels);