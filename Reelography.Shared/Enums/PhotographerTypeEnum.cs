using System.ComponentModel;

namespace Reelography.Shared.Enums;

public enum PhotographerTypeEnum
{
    [Description(
        "Professionals are Studio–Verified business, specialising in premium events " +
        "(weddings, pre‑wedding shoots, large celebrations). Includes at least one " +
        "Google‑Maps‑listed studio address for on‑site consultations."
    )]
    Professional = 1,

    [Description(
        "Freelance Creator–Independent photographer who works on location or from a home setup. " +
        "Skill‑verified by our platform; studio address on Google Maps is optional."
    )]
    Freelancer
}