import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { api } from "../axios/api";
import Headbar from "../Components/Headbar";

interface Language {
    id: number,
    languageName: string,
    flag: string
}

interface WordPair {
    id: number,
    firstWord: string,
    secondWord: string
}

interface Wordset {
    id: number,
    name: string,
    firstLanguage: Language,
    secondLanguage: Language,
    wordPairs: WordPair[]
}

export default function LearnWordset() {
    const [wordset, setWordset] = useState<Wordset | null>(null);
    const [currentCardIndex, setCurrentCardIndex] = useState(0);
    const [showSecondWord, setShowSecondWord] = useState(false);
    const wordsetId = useParams().wordsetId;

    useEffect(() => {
        async function getWordset() {
            try {
                console.log(wordsetId)
                const response = await api.get(`/api/wordset/${wordsetId}`);
                const data = await response.data;
                setWordset(data);
            } catch (error) {
                console.error(error);
            }
        }
        getWordset();
    }, [wordsetId]);

    async function handleFlipCard() {
        setShowSecondWord(!showSecondWord);
    }

    async function handleNextCard() {
        if (wordset && currentCardIndex < wordset.wordPairs.length - 1) {
            setCurrentCardIndex(currentCardIndex + 1);
            setShowSecondWord(false);
        }
    }

    async function handlePrevCard() {
        if (currentCardIndex > 0) {
            setCurrentCardIndex(currentCardIndex - 1);
            setShowSecondWord(false);
        }
    }

    if (!wordset || !wordset.wordPairs || wordset.wordPairs.length === 0) {
        return <div className="text-center p-4">There isn't any wordpair added to this set.</div>;
    }

    const currentCard = wordset.wordPairs[currentCardIndex];

    return (
        <>
            <Headbar />
            <div className="max-w-3xl mx-auto pt-16">
                <div className="mb-6">
                    <h1 className="text-2xl font-bold mb-2">{wordset.name}</h1>
                    <div className="flex items-center gap-4">
                        <div className="flex items-center">
                            <img src={wordset.firstLanguage.flag} alt={wordset.firstLanguage.languageName} className="w-6 h-4 mr-2" />
                            <span>{wordset.firstLanguage.languageName}</span>
                        </div>
                        <span>---</span>
                        <div className="flex items-center">
                            <img src={wordset.secondLanguage.flag} alt={wordset.secondLanguage.languageName} className="w-6 h-4 mr-2" />
                            <span>{wordset.secondLanguage.languageName}</span>
                        </div>
                    </div>
                </div>
                <div className="mb-6">
                    <div className="flex justify-between items-center mb-2 text-sm">
                        <span>Card: {currentCardIndex + 1}/{wordset.wordPairs.length}</span>
                    </div>
                    <div className="bg-white border rounded-lg p-8 shadow-md cursor-pointer min-h-64 flex flex-col justify-center items-center" onClick={handleFlipCard} >
                        {!showSecondWord ? (
                            <div className="text-3xl font-medium">{currentCard.firstWord}</div>
                        ) : (
                            <div className="text-3xl font-medium">{currentCard.secondWord}</div>
                        )}
                    </div>
                </div>
                <div className="flex justify-between">
                    <button onClick={handlePrevCard} disabled={currentCardIndex === 0} className={`px-4 py-2 rounded ${currentCardIndex === 0 ? "bg-gray-200 text-gray-500 cursor-not-allowed" : "bg-blue-500 text-white hover:bg-blue-600"}`} > Prev </button>
                    <button onClick={handleNextCard} disabled={currentCardIndex === wordset.wordPairs.length - 1} className={`px-4 py-2 rounded ${currentCardIndex === wordset.wordPairs.length - 1 ? "bg-gray-200 text-gray-500 cursor-not-allowed" : "bg-blue-500 text-white hover:bg-blue-600"}`} > Next </button>
                </div>
            </div>
        </>
    )
}