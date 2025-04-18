import { useEffect, useState } from "react"
import BlindedTest from "../Components/PlacementTest/BlindedTest";
import WritingTest from "../Components/PlacementTest/WritingTest";
import CorrectTest from "../Components/PlacementTest/CorrectTest";
import ReadingTest from "../Components/PlacementTest/ReadingTest";
import ChooseNewLanguage from "../Components/ChooseNewLanguage";
import TestResult from "../Components/PlacementTest/TestResult";

export default function PlacementTest() {

    const [stage, setStage] = useState<number>(0);
    const [languageId, setLanguageId] = useState(0);
    const [level, setLevel] = useState("");

    useEffect(() => {
        console.log(languageId);
    }, [languageId])

    const tests: Record<number, React.ReactNode> = {
        0: <ChooseNewLanguage setLanguageId={setLanguageId} languageId={languageId} setStage={setStage} />,
        1: < WritingTest setStage={setStage} languageId={languageId} setLevel={setLevel} />,
        2: <ReadingTest setStage={setStage} languageId={languageId} level={level} setLevel={setLevel} />,
        3: <BlindedTest setStage={setStage} languageId={languageId} />,
        4: <CorrectTest languageId={languageId} />,
        5: <TestResult languageId={languageId} />
    };

    return (
        <>
            {tests[stage]}
        </>
    )
}