import { useEffect, useState } from "react"
import BlindedTest from "../Components/PlacementTest/BlindedTest";
import WritingTest from "../Components/PlacementTest/WritingTest";
import CorrectTest from "../Components/PlacementTest/CorrectTest";
import ReadingTest from "../Components/PlacementTest/ReadingTest";
import ChooseNewLanguage from "../Components/ChooseNewLanguage";

export default function PlacementTest() {

    const [stage, setStage] = useState<number>(0);
    const [languageId, setLanguageId] = useState(0);

    useEffect(() => {
        console.log(languageId);
    }, [languageId])

    const tests: Record<number, React.ReactNode> = {
        0: <ChooseNewLanguage setLanguageId={setLanguageId} />,
        1: < WritingTest setStage={setStage} />,
        2: <ReadingTest />,
        3: <BlindedTest />,
        4: <CorrectTest />
    };

    return (
        <>
            {tests[stage]}
        </>
    )
}