import React from 'react';
import EmptyPlace from './EmptyPlace';

interface TextWithEmptyPlacesProps {
    text: string;
    filledSpaces: { [key: number]: string };
    onRemoveWord: (index: number) => void;
    onDrop: (index: number, word: string) => void;
}

export default function TextWithEmptyPlaces({
    text,
    filledSpaces,
    onRemoveWord,
    onDrop
}: TextWithEmptyPlacesProps) {
    const textParts = text.split("_____");

    return (
        <div className="text-gray-800 leading-relaxed text-lg">
            {textParts.map((part, index) => (
                <React.Fragment key={index}>
                    {part}
                    {index < textParts.length - 1 && (
                        <EmptyPlace
                            index={index}
                            word={filledSpaces[index] || ""}
                            onRemoveWord={onRemoveWord}
                            onDrop={onDrop}

                        />
                    )}
                </React.Fragment>
            ))}
        </div>
    );
}